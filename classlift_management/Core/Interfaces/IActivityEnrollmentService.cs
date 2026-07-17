using Core.Contexts;
using Core.Models;
using Core.Repositories;
using Core.ViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IActivityEnrollmentService
    {
        Task<int> AddRegisteredEnrollmentAsync(int childId, int activityId, string status, User user);

        Task<bool> RemoveRegisteredEnrollmentAsync(int enrollmentId);

        Task<IEnumerable<ActivityEnrollmentViewModel>> GetAllEnrollmentsViewByChildAsync(int childId);

        Task<IEnumerable<ActivityEnrollmentViewModel>> GetEnrollmentsViewByChildAsync(int childId, String status);

        Task<IEnumerable<ActivityEnrollmentViewModel>> GetUpcomingEnrollmentsViewByChildAsync(int childId);

       Task<IEnumerable<ActivityEnrollment>> GetUpcomingEnrollmentsByChildAsync(int childId);

        Task<IEnumerable<ActivityEnrollment>> GetRegisteredEnrollmentsByChildAsync(int childId);

        Task<IEnumerable<ActivityEnrollment>> GetFinishedEnrollmentsByChildAsync(int childId);

        Task<IEnumerable<ActivityEnrollment>> GetCanceledEnrollmentsByChildAsync(int childId);


        Task<IEnumerable<ActivityEnrollment>> UpdateActivityStatusToCompletedAsync();

        Task<IEnumerable<ActivityEnrollment>> UpdateActivityStatusToCompletedAsync(AppDbContext dbContext, CancellationToken cancellationToken);

        Task<bool> UpdateActivityStatusToCanceledAsync(int activityId);

        Task<bool> UpdateActivityStatusToClosedAsync(int activityId);

        Task<bool> UpdateActivityEnrollmentStatusToConfirmedAsync(int enrollmentId);


    }

}
