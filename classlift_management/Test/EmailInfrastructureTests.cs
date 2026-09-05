using Core.Email;
using Microsoft.Extensions.Logging.Abstractions;

namespace Test;

public class EmailOptionsValidatorTests
{
    private readonly EmailOptionsValidator _validator = new();

    [Test]
    public void DisabledEmailDoesNotRequireSmtpSettings()
    {
        var result = _validator.Validate(null, new EmailOptions { Enabled = false });

        Assert.That(result.Succeeded, Is.True);
    }

    [Test]
    public void NonProductionCaptureDoesNotRequireSmtpSettings()
    {
        var validator = new EmailOptionsValidator(requireSmtpSettings: false);

        var result = validator.Validate(null, new EmailOptions { Enabled = true });

        Assert.That(result.Succeeded, Is.True);
    }

    [Test]
    public void EnabledEmailRequiresCompleteValidSettings()
    {
        var result = _validator.Validate(null, new EmailOptions
        {
            Enabled = true,
            Port = 0,
            SenderEmail = "not-an-address",
            TimeoutSeconds = 0
        });

        Assert.Multiple(() =>
        {
            Assert.That(result.Failed, Is.True);
            Assert.That(result.Failures, Has.Some.Contains("Email:Host"));
            Assert.That(result.Failures, Has.Some.Contains("Email:Port"));
            Assert.That(result.Failures, Has.Some.Contains("Email:Username"));
            Assert.That(result.Failures, Has.Some.Contains("Email:Password"));
            Assert.That(result.Failures, Has.Some.Contains("Email:SenderEmail"));
            Assert.That(result.Failures, Has.Some.Contains("Email:SenderName"));
            Assert.That(result.Failures, Has.Some.Contains("Email:TimeoutSeconds"));
        });
    }

    [Test]
    public void EnabledEmailAcceptsCompleteSettings()
    {
        var result = _validator.Validate(null, ValidOptions());

        Assert.That(result.Succeeded, Is.True);
    }

    private static EmailOptions ValidOptions() => new()
    {
        Enabled = true,
        Host = "smtp.example.com",
        Port = 587,
        Username = "smtp-user",
        Password = "secret-from-test-only",
        SenderEmail = "no-reply@example.com",
        SenderName = "ClassLift",
        Security = EmailSecurity.StartTls,
        TimeoutSeconds = 30
    };
}

public class EmailServiceBehaviorTests
{
    [Test]
    public async Task DisabledServiceReportsDisabledWithoutSending()
    {
        var service = new NullEmailService();

        var result = await service.SendAsync(ValidMessage());

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Status, Is.EqualTo(EmailSendStatus.Disabled));
        });
    }

    [Test]
    public async Task DevelopmentServiceCapturesWithoutSmtp()
    {
        var store = new DevelopmentEmailStore();
        var service = new DevelopmentEmailService(
            store,
            NullLogger<DevelopmentEmailService>.Instance);

        var result = await service.SendAsync(ValidMessage());

        Assert.Multiple(() =>
        {
            Assert.That(result.Status, Is.EqualTo(EmailSendStatus.Captured));
            Assert.That(store.Messages, Has.Count.EqualTo(1));
            Assert.That(store.Messages[0].Message.Subject, Is.EqualTo("Test subject"));
        });
    }

    [TestCase("bad-address", "invalid_recipient")]
    [TestCase("", "invalid_recipient")]
    public async Task DevelopmentServiceRejectsInvalidRecipient(
        string recipient,
        string expectedErrorCode)
    {
        var service = new DevelopmentEmailService(
            new DevelopmentEmailStore(),
            NullLogger<DevelopmentEmailService>.Instance);

        var result = await service.SendAsync(ValidMessage() with { To = recipient });

        Assert.Multiple(() =>
        {
            Assert.That(result.Status, Is.EqualTo(EmailSendStatus.Failed));
            Assert.That(result.ErrorCode, Is.EqualTo(expectedErrorCode));
        });
    }

    [Test]
    public void HtmlEncoderEscapesUserDerivedContent()
    {
        var encoded = EmailHtml.Encode("<script>alert('x')</script> & text");

        Assert.Multiple(() =>
        {
            Assert.That(encoded, Does.Not.Contain("<script>"));
            Assert.That(encoded, Does.Contain("&lt;script&gt;"));
            Assert.That(encoded, Does.Contain("&amp; text"));
        });
    }

    private static EmailMessage ValidMessage() => new(
        "parent@example.com",
        "Test subject",
        "<p>Test body</p>",
        "Test body");
}
