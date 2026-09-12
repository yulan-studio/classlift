using Core.Contexts;
using Core.Models;
using Core.Repositories;
using Core.Services;
using Microsoft.EntityFrameworkCore;

namespace Test;

public class GroupCourseRegistrationTests
{
    [TestCase(1, "finish setting up all course sessions")]
    [TestCase(3, "inconsistent session data")]
    public async Task RegistrationIsBlockedWhenConfiguredSessionCountDoesNotMatch(
        int configuredSessionCount,
        string expectedMessage)
    {
        await using var context = CreateContext();
        var data = await SeedCourseAsync(context, configuredSessionCount);
        var service = CreateService(context);

        var exception = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await service.AddRegisteredEnrollmentAsync(
                data.Child.ChildID,
                data.Course.CourseID,
                scheduledHours: 0,
                status: "Registered",
                data.Staff));

        Assert.Multiple(() =>
        {
            Assert.That(exception!.Message, Does.Contain(expectedMessage));
            Assert.That(context.CourseEnrollments.Count(), Is.EqualTo(configuredSessionCount));
            Assert.That(context.CourseEnrollments.Any(e => e.ChildID == data.Child.ChildID), Is.False);
        });
    }

    [Test]
    public async Task RegistrationSucceedsWhenConfiguredSessionCountMatches()
    {
        await using var context = CreateContext();
        var data = await SeedCourseAsync(context, configuredSessionCount: 2);
        var service = CreateService(context);

        var enrollmentId = await service.AddRegisteredEnrollmentAsync(
            data.Child.ChildID,
            data.Course.CourseID,
            scheduledHours: 0,
            status: "Registered",
            data.Staff);

        var registration = await context.CourseEnrollments.FindAsync(enrollmentId);
        Assert.Multiple(() =>
        {
            Assert.That(registration, Is.Not.Null);
            Assert.That(registration!.ChildID, Is.EqualTo(data.Child.ChildID));
            Assert.That(registration.EnrollmentID_Ref, Is.Null);
            Assert.That(registration.Status, Is.EqualTo("Registered"));
        });
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"group-course-registration-{Guid.NewGuid()}")
            .Options;
        return new AppDbContext(options);
    }

    private static CourseEnrollmentService CreateService(AppDbContext context) => new(
        new CourseEnrollmentRepository(context),
        new CourseRepository(context),
        new ChildRepository(context),
        new CoachRepository(context));

    private static async Task<(User Staff, Child Child, Course Course)> SeedCourseAsync(
        AppDbContext context,
        int configuredSessionCount)
    {
        var staff = new User
        {
            Id = 1,
            UserName = "staff@example.com",
            Email = "staff@example.com",
            Role = "Staff"
        };
        var family = new User
        {
            Id = 2,
            UserName = "family@example.com",
            Email = "family@example.com",
            Role = "Child"
        };
        var city = new City { CityID = 1, Name = "Toronto", CreatedBy = staff.Id };
        var specialty = new Specialty
        {
            SpecialtyID = 1,
            Title = "Life skills",
            Description = "Life skills",
            CreatedBy = staff.Id
        };
        var child = new Child
        {
            ChildID = 1,
            UserID = family.Id,
            User = family,
            Name = "Jamie",
            CityID = city.CityID,
            City = city
        };
        var course = new Course
        {
            CourseID = 1,
            Title = "Group course",
            CourseType = "Group",
            SessionCount = 2,
            SpecialtyID = specialty.SpecialtyID,
            Specialty = specialty,
            CreatedBy = staff.Id,
            CreatedByUser = staff,
            CreatedDate = DateTime.UtcNow
        };

        context.Users.AddRange(staff, family);
        context.Cities.Add(city);
        context.Specialties.Add(specialty);
        context.Children.Add(child);
        context.Courses.Add(course);

        for (var index = 0; index < configuredSessionCount; index++)
        {
            context.CourseEnrollments.Add(new CourseEnrollment
            {
                CourseID = course.CourseID,
                Course = course,
                Child = null!,
                Status = index == 0 ? "Completed" : "Open",
                ScheduledAt = DateTime.UtcNow.AddDays(index + 1),
                CreatedBy = staff.Id,
                CreatedByUser = staff
            });
        }

        await context.SaveChangesAsync();
        return (staff, child, course);
    }
}
