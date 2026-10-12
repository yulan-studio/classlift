using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models;

public sealed class ProgressReportDraft
{
    [Key]
    public int ProgressReportDraftId { get; set; }

    public int SessionEnrollmentId { get; set; }
    public int CoachId { get; set; }

    [Required, Column(TypeName = "text")]
    public string WorkedOn { get; set; } = string.Empty;

    [Required, Column(TypeName = "text")]
    public string PositiveResponse { get; set; } = string.Empty;

    [Required, Column(TypeName = "text")]
    public string Difficulty { get; set; } = string.Empty;

    [Required, Column(TypeName = "text")]
    public string ContinueNextTime { get; set; } = string.Empty;

    [Column(TypeName = "text")]
    public string? GeneratedDraft { get; set; }

    [Column(TypeName = "text")]
    public string? EditedDraft { get; set; }

    [Required, MaxLength(20)]
    public string Status { get; set; } = "Draft";

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ApprovedAtUtc { get; set; }
    public string? ModelName { get; set; }
    public string? PromptVersion { get; set; }
}
