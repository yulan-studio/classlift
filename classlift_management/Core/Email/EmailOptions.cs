namespace Core.Email;

public sealed class EmailOptions
{
    public const string SectionName = "Email";

    public bool Enabled { get; set; }
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string SenderEmail { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public EmailSecurity Security { get; set; } = EmailSecurity.StartTls;
    public int TimeoutSeconds { get; set; } = 30;
}

public enum EmailSecurity
{
    StartTls,
    SslOnConnect
}
