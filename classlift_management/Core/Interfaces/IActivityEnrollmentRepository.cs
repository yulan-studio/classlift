using Core.Contexts;
using Core.Models;
using Core.ViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IActivityEnrollmentRepository
    {
        Task<bool> AddAsync(ActivityEnrollment enrollment);
        Task<bool> RemoveAsync(int enrollmentId);
        //Task<bool> RemoveRegisteredAsync(int enrollmentId);

        //Task<IEnumerable<ActivityEnrollment>> GetAllEnrollmentsByChildAsync(int childId);

        Task<IEnumerable<ActivityEnrollmentViewModel>> GetUpcomingEnrollmentsViewByChildAsync(int childId);

        Task<IEnumerable<ActivityEnrollment>> GetUpcomingEnrollmentsByChildAsync(int childId);

        Task<IEnumerable<ActivityEnrollment>> GetPastEnrollmentsByChildAsync(int childId);
        
        Task<IEnumerable<ActivityEnrollment>> GetEnrollmentsByChildAsync(int childId, string status);

        Task<IEnumerable<ActivityEnrollment>> GetFinishedEnrollmentsByChildAsync(int childId);

        Task<IEnumerable<ActivityEnrollment>> GetRegisteredEnrollmentsByChildAsync(int childId);

        Task<IEnumerable<ActivityEnrollmentViewModel>> GetAllEnrollmentsViewByChildAsync(int childId);

       Task<IEnumerable<ActivityEnrollmentViewModel>> GetEnrollmentsViewByChildAsync(int childId, String status);


        Task<IEnumerable<ActivityEnrollment>> UpdateActivityStatusToCompletedAsync();

        Task<IEnumerable<ActivityEnrollment>> UpdateActivityStatusToCompletedAsync(AppDbContext dbContext, CancellationToken cancellationToken);

        Task<bool> UpdateActivityStatusToCanceledAsync(int activityId);

        Task<bool> UpdateActivityEnrollmentStatusToConfirmedAsync(int enrollmentId);


        Task<bool> UpdateActivityStatusToClosedAsync(int activityId);

        //Task<CourseEnrollment> GetAsync(int enrollmentId);
        //Task<IEnumerable<CourseEnrollment>> GetEnrollmentsByChildAsync(int childId, string status);
        //Task<IEnumerable<CourseEnrollment>> GetEnrollmentsByCourseAsync(int courseId);
    }
}
