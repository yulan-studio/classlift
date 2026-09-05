using System.ComponentModel.DataAnnotations;
using Core.Contexts;
using Core.Services;
using Core.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Test;

public class OrganizationEmailSettingsViewModelTests
{
    [Test]
    public void ValidEmailAddressesPassValidation()
    {
        var model = new OrganizationEmailSettingsViewModel
        {
            SenderEmail = "no-reply@example.com",
            ReceiverEmail = "notifications@example.com"
        };

        Assert.That(Validate(model), Is.Empty);
    }

    [TestCase("invalid", "notifications@example.com")]
    [TestCase("no-reply@example.com", "invalid")]
    [TestCase("", "notifications@example.com")]
    [TestCase("no-reply@example.com", "")]
    public void InvalidOrMissingEmailAddressFailsValidation(
        string senderEmail,
        string receiverEmail)
    {
        var model = new OrganizationEmailSettingsViewModel
        {
            SenderEmail = senderEmail,
            ReceiverEmail = receiverEmail
        };

        Assert.That(Validate(model), Is.Not.Empty);
    }

    private static List<ValidationResult> Validate(object model)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(
            model,
            new ValidationContext(model),
            results,
            validateAllProperties: true);
        return results;
    }
}

public class OrganizationEmailSettingsServiceTests
{
    [Test]
    public async Task SaveCreatesAndThenUpdatesTheSingleTenantSettingsRow()
    {
        var databaseName = $"organization-email-settings-{Guid.NewGuid()}";
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        await using var dbContext = new AppDbContext(options);
        var service = new OrganizationEmailSettingsService(dbContext);

        await service.SaveAsync("first@example.com", "inbox@example.com");
        var created = await service.GetAsync();
        var createdAt = created!.CreatedAtUtc;

        await service.SaveAsync("second@example.com", "office@example.com");
        var updated = await service.GetAsync();

        Assert.Multiple(() =>
        {
            Assert.That(updated, Is.Not.Null);
            Assert.That(updated!.OrganizationEmailSettingsId, Is.EqualTo(1));
            Assert.That(updated.SenderEmail, Is.EqualTo("second@example.com"));
            Assert.That(updated.ReceiverEmail, Is.EqualTo("office@example.com"));
            Assert.That(updated.CreatedAtUtc, Is.EqualTo(createdAt));
            Assert.That(updated.UpdatedAtUtc, Is.GreaterThanOrEqualTo(createdAt));
        });

        Assert.That(await dbContext.OrganizationEmailSettings.CountAsync(), Is.EqualTo(1));
    }
}
