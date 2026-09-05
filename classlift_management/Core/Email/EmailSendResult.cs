namespace Core.Email;

public sealed record EmailSendResult(EmailSendStatus Status, string? ErrorCode = null)
{
    public bool IsSuccess => Status is EmailSendStatus.Sent
        or EmailSendStatus.Captured
        or EmailSendStatus.Disabled;

    public static EmailSendResult Sent() => new(EmailSendStatus.Sent);
    public static EmailSendResult Captured() => new(EmailSendStatus.Captured);
    public static EmailSendResult Disabled() => new(EmailSendStatus.Disabled);
    public static EmailSendResult Failed(string errorCode) => new(EmailSendStatus.Failed, errorCode);
}

public enum EmailSendStatus
{
    Sent,
    Captured,
    Disabled,
    Failed
}
