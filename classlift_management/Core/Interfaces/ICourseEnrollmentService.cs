using Core.DTOs;
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
    public interface ICourseEnrollmentService
    {
        Task<CourseEnrollment> GetAsync(int enrollmentId);

        Task<bool> RemoveAsync(int enrollmentId);
        Task<int> AddRegisteredEnrollmentAsync(int childId, int courseId, decimal scheduledHours, string status, User user);
        Task<bool> RemoveRegisteredEnrollmentAsync(int enrollmentId);

        Task<bool> AddSessionRegisteredEnrollmentAsync(int childId, int courseId, DateTime? scheduledAt, decimal? scheduledHours, string? location, int enrollmentId_Ref, string status, User user);
        Task<IEnumerable<CourseEnrollmentViewModel>> GetRegisteredEnrollmentsByChildAsync(int childId);

        Task<IEnumerable<CourseEnrollment>> GetEnrollmentsByCourseChildAsync(int courseId, int childId);

        Task<IEnumerable<CourseEnrollment>> GetEnrollments2ByCourseChildAsync(int courseId, int childId);

        Task<IEnumerable<CourseEnrollment>> GetUpcomingEnrollmentsByCourseChildAsync(int courseId, int childId);

        Task<IEnumerable<CourseEnrollment>> GetUpcomingEnrollmentsByChildAsync(int childId);

        Task<IEnumerable<CourseEnrollment>> GetFinishedEnrollmentsByChildAsync(int childId);

        

        Task<IEnumerable<CourseEnrollment>> GetScheduledEnrollmentsByCourseAsync(int courseId);

        Task<IEnumerable<CourseEnrollment>> GetRegisteredEnrollmentsByCourseAsync(int courseId);

        Task<IEnumerable<CourseEnrollment>> GetOpenSessionsByCourseAsync(int courseId);

        Task<IEnumerable<CourseEnrollment>> GetClosedSessionsByCourseAsync(int courseId);

        Task<IEnumerable<CourseEnrollment>> GetCanceledSessionsByCourseAsync(int courseId);

        Task<IEnumerable<CourseEnrollment>> GetCompletedSessionsByCourseAsync(int courseId);

        Task<IEnumerable<CourseEnrollment>> GetAllUpcomingSessionsByCourseAsync(int courseId);

        Task<IEnumerable<CourseEnrollment>> GetAllPastSessionsByCourseAsync(int courseId);

        Task<List<int?>> GetRegisteredUpcomingSessionsByCourseAsync(int courseId);

        //Task<IEnumerable<Child>> GetRegisteredChildrenByCoachAsync(int coachId);

        //Task<IEnumerable<Core.ViewModels.RegisteredChild>> GetRegisterationByCoachAsync(int coachId);
        Task<IEnumerable<Core.ViewModels.ChildViewModel>> GetRegisterationByCourseAsync(int courseId);

        Task<bool> ScheduleCourseAsync(int childId, int courseId, DateTime scheduledAt, decimal scheduledHours, string location, int coachId, int enrollmentId_Ref);

        Task<bool> AddSessionToGroupCourseAsync(int courseId, DateTime scheduledAt, decimal scheduledHours, string location, string staffNote, User user);

        Task<bool> UpdateSessionAsync(CourseEnrollment session);

        Task UpdateChildCompletedSessionsAsync(int courseId);

        Task UpdateCompletedSessionsAsync(int courseId);

        Task UpdateChildCanceledSessionsAsync(int courseId, string staffNote);

        Task<bool> RemoveScheduleAsync(int enrollmentId, string coachNote);

        Task<IEnumerable<CourseEnrollment>> GetScheduledSessionsByChildAsync(int childId);

        Task<IEnumerable<CourseEnrollment>> GetScheduledSessionsToConfirmByChildAsync(int childId);

        Task<IEnumerable<PrivateCourseEnrollmentViewModel>> GetPrivateEnrollmentsViewByChildAsync(int childId, String status);

        Task<IEnumerable<CourseEnrollment>> GetRegisteredByCourseChildAsync(int courseId, int childId);

        Task<IEnumerable<CourseEnrollment>> GetSchedulesByCourseChildAsync(int courseId, int childId);

        Task<IEnumerable<CourseEnrollment>> GetCompletesByCourseChildAsync(int courseId, int childId);

        Task<IEnumerable<CourseEnrollment>> GetDeletedByCourseChildAsync(int courseId, int childId);

        //Manually set session to be completed by Coach
        Task<bool> CompleteSessionAsync(int enrollmentId, Decimal actualHours, string coachNote);

        Task<bool> UpdateCompletedCoursesAsync();

        Task<List<int?>> GetChildrenWithRequestsOrConcernsAsync();

        Task<List<int?>> GetChildrenWithConcernsAsync();

        Task<List<int>> GeEnrollmentsWithScheduleConcernsAsync();

        Task<IEnumerable<CourseEnrollment>> GetWaitToCompleteByCourseChildAsync(int courseId, int childId);

        //Task<bool> UpdateCourseStatusToScheduledAsync(int courseId);

        Task<bool> UpdateCourseEnrollmentStatusToConfirmedAsync(int enrollmentId);

        Task<int?> GetEnrollmentIdByChildAndCourseAsync(int courseId, int childId, string status);

        Task<IEnumerable<CalendarSchedule>> GetCoachSchedulesAsync(int coachId);

        Task<bool> UpdateCoachSchedule(UpdateCoachScheduleViewModel vm);

        //Task<IEnumerable<Child>> GetChildrenByCourseAsync(int courseId);

        //Task<IEnumerable<CourseEnrollmentData>> GetSessionsByCourseAsyn(int courseId);

        Task<SessionAttendanceViewModel> GetAttendanceAsync(int courseId);
    }


}
