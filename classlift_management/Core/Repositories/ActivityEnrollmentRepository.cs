using Core.Contexts;
using Core.Interfaces;
using Core.Models;
using Core.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace Core.Repositories
{
    public class ActivityEnrollmentRepository : IActivityEnrollmentRepository
    {
        private readonly AppDbContext _context;

        public ActivityEnrollmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ActivityEnrollment>> GetUpcomingEnrollmentsByChildAsync(int childId)
        {


            var torontoNow = Core.DateTimeHelper.GetTorontoTime();

            return await _context.ActivityEnrollments
                .Include(e => e.Activity)
                //.Include(e => e.Child)
                .Where(e => e.ChildID == childId && e.Activity.ScheduledAt >= torontoNow && e.Status == "Confirmed")
                .OrderBy(e => e.Activity.ScheduledAt)
                .ToListAsync();
        }


       


        public async Task<IEnumerable<ActivityEnrollment>> GetPastEnrollmentsByChildAsync(int childId)
        {
            
            var torontoNow = Core.DateTimeHelper.GetTorontoTime();

            return await _context.ActivityEnrollments
                .Include(e => e.Activity)
                //.Include(e => e.Child)
                .Where(e => e.ChildID == childId && e.Activity.ScheduledAt < torontoNow)
                .OrderBy(e => e.Activity.ScheduledAt)
                .ToListAsync();
        }


       
        public async Task<IEnumerable<ActivityEnrollmentViewModel>> GetAllEnrollmentsViewByChildAsync(int childId)
        {
           
            return await _context.ActivityEnrollments
           .Include(e => e.Activity)
           .Where(e => e.ChildID == childId)
           .Select(e => new ActivityEnrollmentViewModel
           {

               ActivityID = e.ActivityID,

               ChildID = e.ChildID,
               EnrollmentID = e.EnrollmentID,

               Title = e.Activity.Title,
               Address = e.Activity.Address,
               ScheduledAt = e.Activity.ScheduledAt,
               Description = e.Activity.Description,
               Status = e.Status,
             
               TotalCost = _context.Fees
                        .Where(f => f.ActivityEnrollmentID == e.EnrollmentID)
                        .Select(f => f.TotalCost)
                        .FirstOrDefault(),

               PaymentModel = _context.Fees
                        .Where(f => f.ActivityEnrollmentID == e.EnrollmentID)
                        .Select(f => f.PaymentModel)
                        .FirstOrDefault(),

               PaymentDescription = _context.Fees
                        .Where(f => f.ActivityEnrollmentID == e.EnrollmentID)
                        .Select(f => f.Description)
                        .FirstOrDefault(),
               IsPaid = _context.Fees
                      .Where(f => f.ActivityEnrollmentID == e.EnrollmentID)
                      .Select(f => f.IsPaid)
                       .FirstOrDefault()

           })
           .OrderBy(e => e.ScheduledAt)
           .ToListAsync();

        }


        public async Task<IEnumerable<ActivityEnrollmentViewModel>> GetEnrollmentsViewByChildAsync(int childId, String status)
        {

            return await _context.ActivityEnrollments
           .Include(e => e.Activity)
           .Where(e => e.ChildID == childId && e.Status == status)
           .Select(e => new ActivityEnrollmentViewModel
           {

               ActivityID = e.ActivityID,

               ChildID = e.ChildID,
               EnrollmentID = e.EnrollmentID,

               Title = e.Activity.Title,
               Address = e.Activity.Address,
               ScheduledAt = e.Activity.ScheduledAt,
               Description = e.Activity.Description,
               Status = e.Status,

               TotalCost = _context.Fees
                        .Where(f => f.ActivityEnrollmentID == e.EnrollmentID)
                        .Select(f => f.TotalCost)
                        .FirstOrDefault(),
               PaymentDescription = _context.Fees
                        .Where(f => f.ActivityEnrollmentID == e.EnrollmentID)
                        .Select(f => f.Description)
                        .FirstOrDefault(),
               PaymentModel = _context.Fees
                        .Where(f => f.ActivityEnrollmentID == e.EnrollmentID)
                        .Select(f => f.PaymentModel)
                        .FirstOrDefault()
           })
           .OrderBy(e => e.ScheduledAt)
           .ToListAsync();



        }



        public async Task<IEnumerable<ActivityEnrollment>> GetEnrollmentsByChildAsync(int childId, string status)
        {
            return await _context.ActivityEnrollments
                .Include(e => e.Activity)
                //.Include(e => e.Child)
                .Where(e => e.ChildID == childId && e.Status == status)
                .OrderBy(e => e.Activity.ScheduledAt)
                .ToListAsync();
        }


        public async Task<IEnumerable<ActivityEnrollment>> GetFinishedEnrollmentsByChildAsync(int childId)
        {
            var torontoNow = Core.DateTimeHelper.GetTorontoTime();

            return await _context.ActivityEnrollments
           .Include(e => e.Activity)



           .Where(e => e.ChildID == childId
                      && (e.Status == "Completed" || e.Status == "Canceled")
                      && e.Activity.ScheduledAt < torontoNow)
           
          
           .OrderBy(e => e.Activity.ScheduledAt)
           .ToListAsync();
        }

        public async Task<IEnumerable<ActivityEnrollmentViewModel>> GetUpcomingEnrollmentsViewByChildAsync(int childId)
        {
            var torontoNow = Core.DateTimeHelper.GetTorontoTime();

            return await _context.ActivityEnrollments
           .Include(e => e.Activity)
           


           .Where(e => e.ChildID == childId 
                      && (e.Status == "Registered" || e.Status == "Canceled") 
                      && e.Activity.ScheduledAt > torontoNow )  
           .OrderBy(e => e.CreatedDate)
           .Select(e => new ActivityEnrollmentViewModel
           {

               ActivityID = e.ActivityID,
            
               ChildID = e.ChildID,
               EnrollmentID = e.EnrollmentID,

               Title = e.Activity.Title,
               Address = e.Activity.Address,
               ScheduledAt = e.Activity.ScheduledAt,
               Description = e.Activity.Description,
               Status = e.Status,
                                                                                                                                                                                                                          // NEW: get IsPaid from Fees
               //IsPaid = _context.Fees
               //         .Where(f => f.ActivityEnrollmentID == e.EnrollmentID)
               //         .Select(f => f.IsPaid)
               //         .FirstOrDefault(),
               TotalCost = _context.Fees
                        .Where(f => f.ActivityEnrollmentID == e.EnrollmentID)
                        .Select(f => f.TotalCost)
                        .FirstOrDefault(),
               PaymentModel = _context.Fees
                        .Where(f => f.ActivityEnrollmentID == e.EnrollmentID)
                        .Select(f => f.PaymentModel)
                        .FirstOrDefault()
           })
           .OrderBy(e => e.ActivityID)
           .ToListAsync();
        }

        public async Task<IEnumerable<ActivityEnrollment>> GetRegisteredEnrollmentsByChildAsync(int childId)
        {
            var torontoNow = Core.DateTimeHelper.GetTorontoTime();

            return await _context.ActivityEnrollments
                .Include(e => e.Activity)
                //.Include(e => e.Child)
                .Where(e => e.ChildID == childId && e.Activity.ScheduledAt >= torontoNow && e.Status == "Registered")
                .OrderBy(e => e.Activity.ScheduledAt)
                .ToListAsync();
        }

        public async Task<bool> AddAsync(ActivityEnrollment enrollment)
        {
            //_context.Attach(enrollment.Child);
            await _context.ActivityEnrollments.AddAsync(enrollment);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveAsync(int enrollmentId)
        {
            var enrollment = await _context.ActivityEnrollments.FindAsync(enrollmentId);
            if (enrollment == null) return false;

            _context.ActivityEnrollments.Remove(enrollment);
            return await _context.SaveChangesAsync() > 0;
        }

      

        public async Task<ActivityEnrollment> GetAsync(int enrollmentId)
        {
            return await _context.ActivityEnrollments
                // .Include(e => e.Child)
                .Include(e => e.Activity)
                .FirstOrDefaultAsync(e => e.EnrollmentID == enrollmentId);
        }


        public async Task<IEnumerable<ActivityEnrollment>> UpdateActivityStatusToCompletedAsync()
        {
            var torontoNow = Core.DateTimeHelper.GetTorontoTime();

            
            var enrollments = await _context.ActivityEnrollments
                .Include(e => e.Activity)
                .Where(e => ((DateTime)e.Activity.ScheduledAt).AddDays(1)  <= torontoNow && e.Status == "Confirmed")
                .ToListAsync();

            foreach (var enrollment in enrollments)
            {
                enrollment.Status = "Completed";
            }

            var changes = await _context.SaveChangesAsync();
            return enrollments;
        }


        //For backend service to call
        public async Task<IEnumerable<ActivityEnrollment>> UpdateActivityStatusToCompletedAsync(AppDbContext dbContext,CancellationToken cancellationToken)
        {
            var torontoNow = Core.DateTimeHelper.GetTorontoTime();


            var enrollments = await dbContext.ActivityEnrollments
                .Include(e => e.Activity)
                .Where(e => ((DateTime)e.Activity.ScheduledAt).AddDays(1) <= torontoNow && e.Status == "Confirmed")
                .ToListAsync(cancellationToken);

            foreach (var enrollment in enrollments)
            {
                enrollment.Status = "Completed";
            }

            var changes = await _context.SaveChangesAsync(cancellationToken);
            return enrollments;
        }




        public async Task<bool> UpdateActivityStatusToCanceledAsync(int activityId)
        {

            var enrollments = await _context.ActivityEnrollments
                .Where(e => e.Status == "Scheduled" && e.Activity.ActivityID == activityId)
                .ToListAsync();

            foreach (var enrollment in enrollments)
            {
                enrollment.Status = "Canceled";
            }

            var changes = await _context.SaveChangesAsync();
            return changes >= 0; // Returns true even if 0 rows were affected
        }



        public async Task<bool> UpdateActivityEnrollmentStatusToConfirmedAsync(int enrollmentID)
        {

            // Find the activity enrollment record by ID
            var enrollment = await _context.ActivityEnrollments
                .FirstOrDefaultAsync(e => e.EnrollmentID == enrollmentID);

            if (enrollment == null)
            {
                return false; // Enrollment not found
            }

            // Update status to "Scheduled"
            enrollment.Status = "Confirmed";

            // Update the record in the database
            _context.ActivityEnrollments.Update(enrollment);

            // Save changes
            var result = await _context.SaveChangesAsync();

            // Return true if at least one record was affected
            return result > 0;
        }


        public async Task<bool> UpdateActivityStatusToClosedAsync(int activityId)
        {
            var activity = await _context.Activities.FindAsync(activityId);
            if (activity == null)
                return false;
            

            var enrollments = await _context.ActivityEnrollments
                .Where(e => e.Status == "Scheduled" && e.Activity.ActivityID == activityId)
                .ToListAsync();

            if (enrollments.Count == activity.MaxCapacity)
            {
                activity.Status = "Closed";
            }
            var changes = await _context.SaveChangesAsync();
            return changes >= 0; // Returns true even if 0 rows were affected


        }

        //public async Task<IEnumerable<ActivityEnrollment>> GetEnrollmentsByChildAsync(int childId, string status)
        //{

        //    return await _context.ActivityEnrollments
        //        .Include(e => e.Activity)
        //        //.Include(e => e.Course.Coach)
        //        .Where(e => e.ChildID == childId && e.Status == status)
        //        .ToListAsync();
        //}

        //public async Task<IEnumerable<CourseEnrollment>> GetEnrollmentsByCourseAsync(int courseId)
        //{
        //    return await _context.CourseEnrollments
        //        //.Include(e => e.Child)
        //        .Where(e => e.CourseID == courseId)
        //        .ToListAsync();
        //}
    }
}
