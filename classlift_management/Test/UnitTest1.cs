using Core.Models;
using Core.Repositories;
using Core.Services;

namespace Test;

public class AuditTimestampTests
{
    [Test]
    public void ModelAuditDefaultsUseUtc()
    {
        var beforeCreation = DateTime.UtcNow;

        var auditTimestamps = new (DateTime Created, DateTime Updated)[]
        {
            GetAuditTimestamps(new ChildBalance { TransactionType = "Test" }),
            GetAuditTimestamps(new CourseEnrollment { Status = "Test" }),
            GetAuditTimestamps(new Parent { Name = "Test" }),
            GetAuditTimestamps(new ParentChild { Relationship = "Test" }),
            GetAuditTimestamps(new PaymentPackage { Title = "Test", Description = "Test" }),
            GetAuditTimestamps(new User { Role = "Test" })
        };

        var afterCreation = DateTime.UtcNow;

        foreach (var (created, updated) in auditTimestamps)
        {
            Assert.Multiple(() =>
            {
                Assert.That(created.Kind, Is.EqualTo(DateTimeKind.Utc));
                Assert.That(updated.Kind, Is.EqualTo(DateTimeKind.Utc));
                Assert.That(created, Is.InRange(beforeCreation, afterCreation));
                Assert.That(updated, Is.InRange(beforeCreation, afterCreation));
            });
        }
    }

    private static (DateTime Created, DateTime Updated) GetAuditTimestamps(
        ChildBalance entity) =>
        (entity.CreatedDate, entity.UpdatedDate);

    private static (DateTime Created, DateTime Updated) GetAuditTimestamps(
        CourseEnrollment entity) =>
        (entity.CreatedDate, entity.UpdatedDate);

    private static (DateTime Created, DateTime Updated) GetAuditTimestamps(
        Parent entity) =>
        (entity.CreatedDate!.Value, entity.UpdatedDate!.Value);

    private static (DateTime Created, DateTime Updated) GetAuditTimestamps(
        ParentChild entity) =>
        (entity.CreatedDate, entity.UpdatedDate);

    private static (DateTime Created, DateTime Updated) GetAuditTimestamps(
        PaymentPackage entity) =>
        (entity.CreatedDate, entity.UpdatedDate);

    private static (DateTime Created, DateTime Updated) GetAuditTimestamps(
        User entity) =>
        (entity.CreatedDate!.Value, entity.UpdatedDate!.Value);
}

public class TimeZoneServiceTests
{
    private readonly TimeZoneService _service = new();

    [TestCase("America/Toronto", 13)]
    [TestCase("America/Vancouver", 16)]
    public void ConvertsLocalScheduleToUtc(string zoneId, int expectedUtcHour)
    {
        var local = new DateTime(2026, 7, 15, 9, 0, 0, DateTimeKind.Unspecified);
        var utc = _service.ConvertLocalToUtc(local, zoneId);

        Assert.Multiple(() =>
        {
            Assert.That(utc.Kind, Is.EqualTo(DateTimeKind.Utc));
            Assert.That(utc, Is.EqualTo(new DateTime(2026, 7, 15, expectedUtcHour, 0, 0, DateTimeKind.Utc)));
            Assert.That(_service.ConvertUtcToLocal(utc, zoneId), Is.EqualTo(local));
        });
    }

    [TestCase(2026, 3, 8, 2, 30, "does not exist")]
    [TestCase(2026, 11, 1, 1, 30, "occurs twice")]
    public void RejectsInvalidOrAmbiguousTorontoTimes(
        int year, int month, int day, int hour, int minute, string expectedMessage)
    {
        var local = new DateTime(year, month, day, hour, minute, 0, DateTimeKind.Unspecified);
        var exception = Assert.Throws<ArgumentException>(
            () => _service.ConvertLocalToUtc(local, "America/Toronto"));
        Assert.That(exception!.Message, Does.Contain(expectedMessage));
    }

    [Test]
    public void NewUsersDefaultToToronto()
    {
        Assert.That(new User { Role = "Test" }.TimeZoneId, Is.EqualTo(TimeZoneService.DefaultTimeZoneId));
    }
}

public class GroupRegistrationConfirmationDeadlineTests
{
    [Test]
    public void DeadlineIsMidnightAtStartOfFirstSessionDateInLocalTimeZone()
    {
        var scheduledLocal = new DateTime(2026, 7, 15, 9, 0, 0, DateTimeKind.Unspecified);
        var scheduledUtc = new DateTime(2026, 7, 15, 13, 0, 0, DateTimeKind.Utc);

        var deadline = CourseEnrollmentRepository.GetGroupRegistrationConfirmationDeadlineUtc(
            scheduledUtc,
            scheduledLocal,
            "America/Toronto");

        Assert.That(deadline, Is.EqualTo(
            new DateTime(2026, 7, 15, 4, 0, 0, DateTimeKind.Utc)));
    }

    [Test]
    public void LegacySessionUsesMidnightAtStartOfUtcSessionDate()
    {
        var scheduledUtc = new DateTime(2026, 7, 15, 13, 0, 0, DateTimeKind.Utc);

        var deadline = CourseEnrollmentRepository.GetGroupRegistrationConfirmationDeadlineUtc(
            scheduledUtc,
            scheduledLocalTime: null,
            timeZoneId: null);

        Assert.That(deadline, Is.EqualTo(
            new DateTime(2026, 7, 15, 0, 0, 0, DateTimeKind.Utc)));
    }
}
