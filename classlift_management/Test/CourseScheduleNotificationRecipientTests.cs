using Core.Contexts;
using Core.Models;
using Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Test;

public class CourseScheduleNotificationRecipientTests
{
    [Test]
    public async Task ReturnsOnlyNonDeletedFamiliesLinkedToTheEditedMasterSession()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"schedule-notification-recipients-{Guid.NewGuid()}")
            .Options;

        await using var context = new AppDbContext(options);
        var familyUser = new User
        {
            Id = 10,
            UserName = "family@example.com",
            Email = "family@example.com",
            Role = "Child"
        };
        var anotherUser = new User
        {
            Id = 11,
            UserName = "another@example.com",
            Email = "another@example.com",
            Role = "Child"
        };
        var firstChild = new Child
        {
            ChildID = 20,
            UserID = familyUser.Id,
            User = familyUser,
            Name = "Jamie",
            City = null!
        };
        var secondChild = new Child
        {
            ChildID = 21,
            UserID = anotherUser.Id,
            User = anotherUser,
            Name = "Morgan",
            City = null!
        };

        context.Users.AddRange(familyUser, anotherUser);
        context.Children.AddRange(firstChild, secondChild);
        context.CourseEnrollments.AddRange(
            ChildSession(101, 50, firstChild, "Scheduled"),
            ChildSession(102, 50, secondChild, "Deleted"),
            ChildSession(103, 99, secondChild, "Scheduled"));
        await context.SaveChangesAsync();

        var repository = new CourseEnrollmentRepository(context);
        var recipients = await repository.GetScheduleNotificationRecipientsAsync(50);

        Assert.Multiple(() =>
        {
            Assert.That(recipients, Has.Count.EqualTo(1));
            Assert.That(recipients[0].ParticipantName, Is.EqualTo("Jamie"));
            Assert.That(recipients[0].Email, Is.EqualTo("family@example.com"));
        });
    }

    private static CourseEnrollment ChildSession(
        int enrollmentId,
        int masterSessionId,
        Child child,
        string status) => new()
        {
            EnrollmentID = enrollmentId,
            EnrollmentID_Ref = masterSessionId,
            ChildID = child.ChildID,
            Child = child,
            CourseID = 1,
            Course = null!,
            Status = status
        };
}
