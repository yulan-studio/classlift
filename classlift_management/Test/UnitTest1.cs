using Core.Models;

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
