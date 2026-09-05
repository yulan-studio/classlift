using System.Collections.Concurrent;

namespace Core.Email;

public sealed class DevelopmentEmailStore
{
    private const int MaximumMessages = 100;
    private readonly ConcurrentQueue<CapturedEmail> _messages = new();

    public IReadOnlyList<CapturedEmail> Messages => _messages.ToArray();

    internal void Add(EmailMessage message)
    {
        _messages.Enqueue(new CapturedEmail(message, DateTimeOffset.UtcNow));

        while (_messages.Count > MaximumMessages)
        {
            _messages.TryDequeue(out _);
        }
    }
}

public sealed record CapturedEmail(EmailMessage Message, DateTimeOffset CapturedAtUtc);
