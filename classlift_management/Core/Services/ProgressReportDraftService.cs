using Core.DTOs;
using Core.Interfaces;
using Core.Models;

namespace Core.Services;

public sealed class TemplateProgressReportGenerator : IProgressReportGenerator
{
    public Task<string> GenerateAsync(ProgressReportInput input, CancellationToken cancellationToken = default)
    {
        var draft = $"Today, the participant worked on: {input.WorkedOn.Trim()}\n\n" +
                    $"Positive response: {input.PositiveResponse.Trim()}\n\n" +
                    $"Difficulty observed: {input.Difficulty.Trim()}\n\n" +
                    $"Suggested continuation: {input.ContinueNextTime.Trim()}";
        return Task.FromResult(draft);
    }
}

public sealed class ProgressReportDraftService : IProgressReportDraftService
{
    private readonly IProgressReportDraftRepository _repository;
    private readonly IProgressReportGenerator _generator;

    public ProgressReportDraftService(IProgressReportDraftRepository repository, IProgressReportGenerator generator)
    {
        _repository = repository;
        _generator = generator;
    }

    public async Task<ProgressReportDraftResult> GenerateDraftAsync(
        ProgressReportInput input,
        CancellationToken cancellationToken = default)
    {
        Validate(input);

        var session = await _repository.GetSessionAsync(input.SessionEnrollmentId, cancellationToken);

        if (session?.ChildID == null || session.Course?.CoachID != input.CoachId)
            throw new UnauthorizedAccessException("The Coach is not assigned to this session.");

        if (session.Status is "Canceled" or "Deleted" || session.Status != "Completed")
            throw new InvalidOperationException("A report draft can only be generated for a completed session.");

        var draft = await _repository.GetCurrentDraftAsync(input.SessionEnrollmentId, cancellationToken);

        var generated = await _generator.GenerateAsync(input, cancellationToken);
        if (draft == null)
        {
            draft = new ProgressReportDraft
            {
                SessionEnrollmentId = input.SessionEnrollmentId,
                CoachId = input.CoachId,
                CreatedAtUtc = DateTime.UtcNow
            };
            _repository.Add(draft);
        }

        draft.WorkedOn = input.WorkedOn.Trim();
        draft.PositiveResponse = input.PositiveResponse.Trim();
        draft.Difficulty = input.Difficulty.Trim();
        draft.ContinueNextTime = input.ContinueNextTime.Trim();
        draft.GeneratedDraft = generated;
        draft.EditedDraft = generated;
        draft.Status = "Draft";
        draft.ModelName = "template";
        draft.PromptVersion = "progress-report-v1";
        await _repository.SaveChangesAsync(cancellationToken);

        return new ProgressReportDraftResult(draft.ProgressReportDraftId, generated, false);
    }

    public async Task<bool> ApproveAsync(int draftId, int coachId, string finalText, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(finalText) || finalText.Length > 12000)
            return false;

        var draft = await _repository.GetDraftForCoachAsync(draftId, coachId, cancellationToken);
        if (draft == null || draft.Status == "Approved")
            return false;

        var session = await _repository.GetCompletedSessionAsync(draft.SessionEnrollmentId, cancellationToken);
        if (session == null)
            return false;

        session.CoachNote = finalText.Trim();
        session.UpdatedDate = DateTime.UtcNow;
        draft.EditedDraft = session.CoachNote;
        draft.Status = "Approved";
        draft.ApprovedAtUtc = DateTime.UtcNow;
        await _repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static void Validate(ProgressReportInput input)
    {
        if (input.SessionEnrollmentId <= 0 || input.CoachId <= 0 ||
            string.IsNullOrWhiteSpace(input.WorkedOn) ||
            string.IsNullOrWhiteSpace(input.PositiveResponse) ||
            string.IsNullOrWhiteSpace(input.Difficulty) ||
            string.IsNullOrWhiteSpace(input.ContinueNextTime))
            throw new ArgumentException("All four progress observations are required.");
    }
}
