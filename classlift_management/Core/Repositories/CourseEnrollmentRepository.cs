using Core.Contexts;
using Core.DTOs;
using Core.DTOs.Report;
using Core.Interfaces;
using Core.Models;
using Core.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace Core.Repositories
{
    public class CourseEnrollmentRepository : ICourseEnrollmentRepository
    {
        private readonly AppDbContext _context;

        public CourseEnrollmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(CourseEnrollment enrollment)
        {
            //_context.Attach(enrollment.Child);
            await _context.CourseEnrollments.AddAsync(enrollment);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveAsync(int enrollmentId)
        {
            var enrollment = await _context.CourseEnrollments.FindAsync(enrollmentId);
            if (enrollment == null) return false;

            _context.CourseEnrollments.Remove(enrollment);
            return await _context.SaveChangesAsync() > 0;
        }

      

        public async Task<CourseEnrollment> GetAsync(int enrollmentId)
        {
            return await _context.CourseEnrollments
               // .Include(e => e.Child)
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.EnrollmentID == enrollmentId);
        }

        public async Task<bool> UpdateAsync(CourseEnrollment entity)
        {
            try
            {
                _context.CourseEnrollments.Update(entity);
                await _context.SaveChangesAsync();  // Commit the changes asynchronously
                return true;
            }
            catch (Exception ex)
            {
                return false; // Return failure in case of an exception
            }
        }


        public async Task<bool> DeleteAsync(CourseEnrollment entity)
        {
            try
            {
                _context.CourseEnrollments.Remove(entity);
                await _context.SaveChangesAsync();  // Commit the changes asynchronously
                return true;
            }
            catch (Exception ex)
            {
                return false; // Return failure in case of an exception
            }
        }

        //Get Registered/Scheduled/completed/request to schedule/request to leave/deleted group or private course session
        public async Task<IEnumerable<CourseEnrollment>> GetEnrollmentsByChildAsync(int childId, string status)
        {
            return await _context.CourseEnrollments
                .Include(e => e.Course)
                .Include(e => e.Course.Coach)
                .Include(e => e.Course.Specialty)
                .Where(e => e.ChildID != null &&e.ChildID == childId && e.Status == status && e.EnrollmentID_Ref != null)
                .OrderBy(e => e.CourseID)
                .OrderBy(e => e.ScheduledAt)
                .ToListAsync();
        }


        public async Task<IEnumerable<CourseEnrollment>> GetFinishedEnrollmentsByChildAsync(int childId)
        {
            var torontoNow = Core.DateTimeHelper.GetTorontoTime();

            return await _context.CourseEnrollments
                .Include(e => e.Course)
                .Include(e => e.Course.Coach)
                .Include(e => e.Course.Specialty)
                .Where(e => e.ChildID != null && e.ChildID == childId && (e.Status == "Completed" || e.Status == "Canceled" || e.Status == "OnLeave") && e.EnrollmentID_Ref != null && ((DateTime)e.ScheduledAt).AddHours((double)e.ScheduledHours) <= torontoNow)
                .OrderBy(e => e.CourseID)
                .OrderBy(e => e.ScheduledAt)
                .ToListAsync();
        }

        //Get Registered/Completed records of root course registration
        public async Task<IEnumerable<CourseEnrollment>> GetRootEnrollmentsByChildAsync(int childId, string status)
        {
            return await _context.CourseEnrollments
                .Include(e => e.Course)
                .Include(e => e.Course.Coach)
                .Include(e => e.Course.Specialty)
                .Where(e => e.ChildID != null && e.ChildID == childId && e.Status == status)
                .OrderBy(e => e.CourseID)
                .OrderBy(e => e.ScheduledAt)
                .ToListAsync();
        }
        //Return Group Course Schedule/Deleted sessions, this is the status of the group sessions before confirming
        public async Task<IEnumerable<CourseEnrollment>> GetScheduledSessionsToConfirmByChildAsync(int childId)
        {
            return await _context.CourseEnrollments
                .Include(e => e.Course)
                .Include(e => e.Course.Coach)
                .Include(e => e.Course.Specialty)
                .Where(e => e.ChildID != null && e.ChildID == childId && (e.Status == "Registered") && e.Course.CourseType=="Group" && e.EnrollmentID_Ref != null)
                .OrderBy(e => e.CourseID)
                .OrderBy(e => e.ScheduledAt)
                .ToListAsync();
        }


        public async Task<IEnumerable<CourseEnrollment>> GetEnrollmentsByChildAsync(int childId)
        {
            return await _context.CourseEnrollments
                .Include(e => e.Course)
                .Include(e => e.Course.Coach)
                .Include(e => e.Course.Specialty)
                .Where(e => e.ChildID != null && e.ChildID == childId &&  e.EnrollmentID_Ref != null)
                .OrderBy(e => e.CourseID)
                .OrderBy(e => e.ScheduledAt)
                .ToListAsync();
        }

        //Include /Scheduled/RequestToReschedule/RequestToCancel/Canceled/Deleted (not include Registered, Completed )  for private course
        //Include /Scheduled/RequestToReschedule/RequestToCancel/Canceled (not include Registered, Completed, Deleted ) for group courses
        public async Task<IEnumerable<CourseEnrollment>> GetUpcomingEnrollmentsByChildAsync(int childId)
        {
            var torontoNow = Core.DateTimeHelper.GetTorontoTime();
            return await _context.CourseEnrollments
                .Include(e => e.Course)
                .Include(e => e.Course.Coach)
                .Include(e => e.Course.Specialty)
                .Where(e => e.ChildID != null && e.ChildID == childId && e.EnrollmentID_Ref != null && (e.Status != "Registered" && e.Status != "Completed" && e.Status != "Deleted"|| (e.Course.CourseType == "Private" && e.Status == "Deleted"))&& ((DateTime)e.ScheduledAt).AddHours((double)e.ScheduledHours) >= torontoNow)
                .OrderBy(e => e.CourseID)
                .OrderBy(e => e.ScheduledAt)
                .ToListAsync();
        }


        //Get registered and completed course information, so that these course won't be registered again
        //Get Registered courses for a child, include number of scheduled sessions, number of completed sessions, number of canceled sessions, number of on leave sessions, number of request to reschedule sessions, number of request to leave sessions
        public async Task<IEnumerable<CourseEnrollmentViewModel>> GetRegisteredEnrollmentsByChildAsync(int childId)  
        {
            return await _context.CourseEnrollments
           .Include(e => e.Course)
           .Include(e => e.Course.Coach)
           .Include(e => e.Course.Specialty)
           
           
           .Where(e => e.ChildID == childId && ((e.Status == "Registered" || e.Status == "Confirmed" || e.Status == "Scheduled" || e.Status == "Completed" ) && e.EnrollmentID_Ref == null))  //Not included those registered to session
           .OrderBy(e => e.CreatedDate)
           .Select(e => new CourseEnrollmentViewModel
           {
               
               CourseID = e.CourseID,
               CourseType = e.Course.CourseType,
               IsActive = e.Course.IsActive,
               SessionCount = e.Course.SessionCount,
               ChildID = e.ChildID,
               EnrollmentID = e.EnrollmentID,
               
               Title = e.Course.Title,
               CoachName = e.Course.Coach != null ? e.Course.Coach.Name : "N/A",
               SpecialtyName = e.Course.Specialty.Title,
               HourlyCost = e.Course.HourlyCost,
               HourlyCost2 = e.Course.HourlyCost2,
               Status = e.Status,
               RegisteredSessions = _context.CourseEnrollments.Count(c => c.ChildID == e.ChildID && c.CourseID == e.CourseID && c.Status == "Registered" && c.EnrollmentID_Ref != null), // Count all registered sessions
               ScheduledSessions = _context.CourseEnrollments.Count(c => c.ChildID == e.ChildID && c.CourseID == e.CourseID && c.Status == "Scheduled" && c.EnrollmentID_Ref != null), // Count all scheduled sessions
               CompletedSessions = _context.CourseEnrollments.Count(c => c.ChildID == e.ChildID && c.CourseID == e.CourseID && c.Status == "Completed" && c.EnrollmentID_Ref != null), // Count completed sessions
               CanceledSessions = _context.CourseEnrollments.Count(c => c.ChildID == e.ChildID && c.CourseID == e.CourseID && c.Status == "Canceled" && c.EnrollmentID_Ref != null), // Count all canceled sessions
               OnLeaveSessions = _context.CourseEnrollments.Count(c => c.ChildID == e.ChildID && c.CourseID == e.CourseID && c.Status == "OnLeave" && c.EnrollmentID_Ref != null), // Count all on leave sessions
               RequestToLeaveSessions = _context.CourseEnrollments.Count(c => c.ChildID == e.ChildID && c.CourseID == e.CourseID && c.Status == "RequestToLeave" && c.EnrollmentID_Ref != null), // Count all requested to leave sessions,
               RequestToRescheduleSessions = _context.CourseEnrollments.Count(c => c.ChildID == e.ChildID && c.CourseID == e.CourseID && c.Status == "RequestToReschedule" && c.EnrollmentID_Ref != null), // Count all requested to leave sessions
                                                                                                                                                                                                           // NEW: get IsPaid from Fees
               IsPaid = _context.Fees
                        .Where(f => f.CourseEnrollmentID == e.EnrollmentID)
                        .Select(f => f.IsPaid)
                        .FirstOrDefault(),
               TotalCost = _context.Fees
                        .Where(f => f.CourseEnrollmentID == e.EnrollmentID)
                        .Select(f => f.TotalCost)
                        .FirstOrDefault(),
               PaymentModel = _context.Fees
                        .Where(f => f.CourseEnrollmentID == e.EnrollmentID)
                        .Select(f => f.PaymentModel)
                        .FirstOrDefault(),
               PaymentDescription = _context.Fees
                        .Where(f => f.CourseEnrollmentID == e.EnrollmentID)
                        .Select(f => f.Description)
                        .FirstOrDefault()
           })
           .OrderBy(e => e.CourseID)
           .ToListAsync();
        }

        //Return all children's enrollments with a course
        public async Task<IEnumerable<CourseEnrollment>> GetEnrollmentsByCourseAsync(int courseId)
        {
            return await _context.CourseEnrollments
                .Include(e => e.Child)
                .Where(e => e.CourseID == courseId && e.EnrollmentID_Ref != null)
                .ToListAsync();
        }

        public async Task<IEnumerable<CourseEnrollment>> GetEnrollmentsByCourseChildAsync(int courseId, int childId, string status)
        {
            return await _context.CourseEnrollments
                .Include(e => e.Child)
                .Include(e => e.Course)
                .Where(e => e.CourseID == courseId && e.ChildID == childId && e.Status == status && e.EnrollmentID_Ref !=null)
                .OrderBy (e => e.ScheduledAt)
                .ToListAsync();
        }


        public async Task<int?> GetEnrollmentIdByChildAndCourseAsync(int courseId, int childId, string status)
        {
            return await _context.CourseEnrollments
               
                .Where(e => e.CourseID == courseId && e.ChildID == childId && e.Status == status && e.EnrollmentID_Ref == null)
                .Select(e => (int?)e.EnrollmentID)
                .FirstOrDefaultAsync();
        }


        public async Task<IEnumerable<CourseEnrollment>> GetOverduedEnrollmentsByCourseChildAsync(int courseId, int childId, string status)
        {
            var torontoNow = Core.DateTimeHelper.GetTorontoTime();

            return await _context.CourseEnrollments
                .Include(e => e.Child)
                .Include(e => e.Course)
                .Where(e => e.CourseID == courseId && 
                            e.ChildID == childId && 
                            e.Status == status && 
                            e.EnrollmentID_Ref != null 
                            && ((DateTime)e.ScheduledAt).AddHours((double)e.ScheduledHours) < torontoNow
                      )
                .OrderBy(e => e.ScheduledAt)
                .ToListAsync();
        }

        //Include Registered/Scheduled/Canceled/Completed/RequestToReschedule/RequestToCancel (not include Deleted)
        public async Task<IEnumerable<CourseEnrollment>> GetEnrollmentsByCourseChildAsync(int courseId, int childId)
        {
            return await _context.CourseEnrollments
                .Include(e => e.Child)
                .Include(e => e.Course)
                .Where(e => e.CourseID == courseId && e.ChildID == childId && e.EnrollmentID_Ref != null && e.Status != "Deleted")
                .OrderBy(e => e.ScheduledAt)
                .ToListAsync();
        }

        //Include Registered/Scheduled/Completed/RequestToReschedule/RequestToCancel (not include Deleted, Canceled)
        public async Task<IEnumerable<CourseEnrollment>> GetEnrollments2ByCourseChildAsync(int courseId, int childId)
        {
            return await _context.CourseEnrollments
                .Include(e => e.Child)
                .Include(e => e.Course)
                .Where(e => e.CourseID == courseId && e.ChildID == childId && e.EnrollmentID_Ref != null && e.Status != "Deleted" && e.Status != "Canceled")
                .OrderBy(e => e.ScheduledAt)
                .ToListAsync();
        }



        //Include /Scheduled/RequestToReschedule/RequestToCancel/Canceled/Deleted (not include Registered, Completed )
        public async Task<IEnumerable<CourseEnrollment>> GetUpcomingEnrollmentsByCourseChildAsync(int courseId, int childId)
        {
            var torontoNow = Core.DateTimeHelper.GetTorontoTime();
            return await _context.CourseEnrollments
                .Include(e => e.Child)
                .Include(e => e.Course)
                .Where(e => e.CourseID == courseId && e.ChildID == childId && e.EnrollmentID_Ref != null && e.Status != "Registered" && e.Status != "Completed" && ((DateTime)e.ScheduledAt).AddHours((double)e.ScheduledHours)>=torontoNow)
                .OrderBy(e => e.ScheduledAt)
                .ToListAsync();
        }





        //public async Task<IEnumerable<CourseEnrollment>> GetEnrollmentsByCoachAsync(int coachId, string status)
        //{
        //    return await _context.CourseEnrollments
        //        .Include(e => e.Child)
        //        .Include(e => e.Child.City)
        //        .Include(e => e.Course.Coach)
        //        .Where(e => e.Course.Coach.CoachID == coachId && e.Status == status)
        //        .OrderByDescending(e => e.CreatedDate)
        //        .ToListAsync();
        //}


        //return the children who registered/canceled/completed courses
        public async Task<IEnumerable<CourseEnrollment>> GetEnrollmentsByCourseAsync(int courseId, string status)
        {
            return await _context.CourseEnrollments
                .Include(e => e.Child)
                .ThenInclude(c => c.City)
                .Where(e => e.CourseID == courseId && e.Status == status && e.EnrollmentID_Ref == null)
                .ToListAsync();
        }

        //open/closed course sessions
        public async Task<IEnumerable<CourseEnrollment>> GetSessionsByCourseAsync(int courseId, string status)
        {
            return await _context.CourseEnrollments
                .Where(e => e.CourseID == courseId && e.Status == status && e.Child==null)
                .OrderBy(e => e.ScheduledAt) // Sort by ScheduledAt ascending
                .ToListAsync();
        }


        //Only return upcoming sessions to delete or edit 
        public async Task<IEnumerable<CourseEnrollment>> GetAllUpcomingSessionsByCourseAsync(int courseId)
        {
            var torontoNow = Core.DateTimeHelper.GetTorontoTime();
            return await _context.CourseEnrollments
                
                .Where(e => e.CourseID == courseId && e.Child==null && e.ScheduledAt >= torontoNow)
                .OrderBy(e => e.ScheduledAt) // Sort by ScheduledAt ascending
                .ToListAsync();
        }

        public async Task<IEnumerable<CourseEnrollment>> GetAllPastSessionsByCourseAsync(int courseId)
        {
            var torontoNow = Core.DateTimeHelper.GetTorontoTime();
            return await _context.CourseEnrollments

                .Where(e => e.CourseID == courseId && e.Child == null && e.ScheduledAt < torontoNow)
                .OrderBy(e => e.ScheduledAt) // Sort by ScheduledAt ascending
                .ToListAsync();
        }


        //Only return List of Upcoming Session IDs in a course that are registered 
        public async Task<List<int?>> GetRegisteredUpcomingSessionsByCourseAsync(int courseId)
        {
            var torontoNow = Core.DateTimeHelper.GetTorontoTime();
            return await _context.CourseEnrollments
                .Where(e => e.CourseID == courseId && e.ChildID != null && e.ScheduledAt >= torontoNow)
                .OrderBy(e => e.ScheduledAt) // Sort by ScheduledAt ascending
                .Select(e => e.EnrollmentID_Ref)
                .ToListAsync();
        }

        //return a list of ChildIDs where at least one course enrollment session has Status == "RequestToLeave":
        public async Task<List<int?>> GetChildrenWithRequestToLeaveAsync()
        {
            return await _context.CourseEnrollments
                .Where(e => e.EnrollmentID_Ref != null && e.Status == "RequestToLeave")
                .Select(e => e.ChildID)
                .Distinct()
                .ToListAsync();
        }


       


        //Here's how you can create a method to return a list of children who:
        //Have a group course session(EnrollmentID_Ref != null)
        //Status is "Registered"
        //ParentNote is not null or empty(indicating a concern about the scheduled time)
        public async Task<List<int?>> GetChildrenWithScheduleConcernsAsync()
        {
            return await _context.CourseEnrollments
                .Where(e =>
                    e.EnrollmentID_Ref != null &&
                    e.Status == "Registered" &&
                    !string.IsNullOrEmpty(e.ParentNote))
                .Select(e => e.ChildID)
                .Distinct()
                .ToListAsync();
        }


        public async Task<List<int>> GetEnrollmentsWithScheduleConcernsAsync()
        {
           
            return await _context.CourseEnrollments
                .Where(e => e.ScheduledAt == null &&
                    _context.CourseEnrollments.Any(f =>
                        f.EnrollmentID_Ref != null &&
                        f.Status == "Registered" &&
                        !string.IsNullOrEmpty(f.ParentNote) &&
                        f.CourseID == e.CourseID &&
                        f.ChildID == e.ChildID))
                .Select(e => e.EnrollmentID)
                .Distinct()
                .ToListAsync();
        }

        //This is for staff to get all children who have either a request to leave or a concern about their schedule
        public async Task<List<int?>> GetChildrenWithRequestsOrConcernsAsync()
        {
            var requestToLeaveChildren = await GetChildrenWithRequestToLeaveAsync();
            var scheduleConcernChildren = await GetChildrenWithScheduleConcernsAsync();

            // Combine both lists and eliminate duplicates
            var combined = requestToLeaveChildren
                .Union(scheduleConcernChildren)
                .ToList();

            return combined;
        }



        //For Group Courses Set children's sessions to be completed when it's finished
        public async Task UpdateChildCompletedSessionsAsync(int courseId)
        {
           
            DateTime now = DateTimeHelper.GetTorontoTime();

           //Get all sessions of the course, which Status is 'Scheduled'
            var sessionsToUpdate = await _context.CourseEnrollments
                .Include(e=>e.Course)
                .Where(e => e.CourseID == courseId
                            && e.Course.CourseType == "Group"
                            && e.Status == "Scheduled"
                            && e.ScheduledAt!=null
                            && e.ScheduledHours != null
                            && ((DateTime)e.ScheduledAt).AddHours((double)e.ScheduledHours) <= now)
                .ToListAsync();

            // Update Status
            foreach (var session in sessionsToUpdate)
            {
                session.Status = "Completed";
                session.ActualHours = session.ScheduledHours; // Assuming actual hours are the same as scheduled hours when completed
            }

            await _context.SaveChangesAsync();
        }



        public async Task<bool> UpdateCourseEnrollmentStatusToConfirmedAsync(int enrollmentID)
        {

            // Find the activity enrollment record by ID
            var enrollment = await _context.CourseEnrollments
                .FirstOrDefaultAsync(e => e.EnrollmentID == enrollmentID);

            if (enrollment == null)
            {
                return false; // Enrollment not found
            }

            // Update status to "Scheduled"
            enrollment.Status = "Confirmed";

            // Update the record in the database
            _context.CourseEnrollments.Update(enrollment);

            // Save changes
            var result = await _context.SaveChangesAsync();

            // Return true if at least one record was affected
            return result > 0;
        }


       



        //For Group Courses, Set sessions to be completed when it's finished
        public async Task UpdateCompletedSessionsAsync(int courseId)
        {

            var torontoNow = Core.DateTimeHelper.GetTorontoTime();

            //Get all sessions of the group course, which Status is 'Scheduled'
            var sessionsToUpdate = await _context.CourseEnrollments
                .Include(e => e.Course)
                //.ThenInclude(c => c.Coach)
                .Where(e => e.CourseID == courseId
                            && (e.Course.CourseType == "Group")
                            && (e.Status == "Open" || e.Status == "Closed")
                            && e.ScheduledAt != null
                            && e.ScheduledHours != null
                            && ((DateTime)e.ScheduledAt).AddHours((double)e.ScheduledHours) <= torontoNow)
                .ToListAsync();

            // Update Status
            foreach (var session in sessionsToUpdate)
            {
                session.Status = "Completed";
                session.ActualHours = session.ScheduledHours; // Assuming actual hours are the same as scheduled hours when completed

                //Add coach hours for this session to CoachIncome table
                var incomeEntry = new CoachIncome
                {
                    CoachID = (int)session.Course.CoachID,
                    CourseID = session.CourseID,
                    EnrollmentID = session.EnrollmentID,
                    //IncomeChange = incomeForThisSession,
                    //Income = newIncome,
                    CreatedDate = DateTimeHelper.GetTorontoTime(),
                    CreatedBy = 0
                };

                _context.CoachIncomes.Add(incomeEntry);
                //await _context.SaveChangesAsync();
            }

            await _context.SaveChangesAsync();
        }


        //Set all registration related to a enrollmentId to be Cancled.
        public async Task UpdateChildCanceledSessionsAsync(int enrollmentId, string staffNote)
        {



            //Get all registered sessions of the course session, which Status is 'Scheduled' or 'Registered'
            var sessionsToUpdate = await _context.CourseEnrollments
                .Where(e => e.EnrollmentID_Ref == enrollmentId
                            && (e.Status == "Scheduled" || e.Status == "Registered")
                            && e.ScheduledAt != null
                            && e.ScheduledHours != null)
                .ToListAsync();

            // Update Status
            foreach (var session in sessionsToUpdate)
            {
                session.Status = "Canceled";
                session.StaffNote = staffNote;
            }

            await _context.SaveChangesAsync();
        }



        public async Task<bool> UpdateCompletedCoursesAsync()
        {
            var updated = false;

            // Step 1: Get all root course enrollments for private and group courses (not per-session enrollments)
            var rootEnrollments = await _context.CourseEnrollments
                .Include(e => e.Course)
                .Where(e =>
                    e.Status == "Confirmed" &&
                    e.EnrollmentID_Ref == null &&
                    e.ScheduledAt == null &&
                    e.Course.SessionCount != null)
                .ToListAsync();
            //Step 2: Get all Completed sessions for all root course, check completed count
            foreach (var root in rootEnrollments)
            {
                int completedSessions = await _context.CourseEnrollments
                    .Where(c =>
                        c.ChildID == root.ChildID &&
                        c.CourseID == root.CourseID &&
                        c.EnrollmentID_Ref != null &&
                        c.Status == "Completed") // We only want to change "registered" sessions to "Completed"
                    .CountAsync();

                //if completed count equal to session count
                if (completedSessions == root.Course.SessionCount)
                {
                    //Step 3: Fetch those registered sessions to update them

                    //var registrationToUpdate = await _context.CourseEnrollments
                    //    .Where(c =>
                    //        c.ChildID == root.ChildID &&
                    //        c.CourseID == root.CourseID &&
                    //        c.EnrollmentID_Ref == null &&
                    //        c.ScheduledAt == null &&
                    //        c.Status == "Registered")
                    //    .ToListAsync();

                    //foreach (var registration in registrationToUpdate)
                    //{
                    //    registration.Status = "Completed";
                    //}

                    root.Status = "Completed";

                    updated = true;
                }
            }

            if (updated)
            {
                await _context.SaveChangesAsync();
            }

            return updated;
        }



        //public async Task<bool> UpdateCourseStatusToConfirmedAsync(int enrollmentID)
        //{

        //    // Find the activity enrollment record by ID
        //    var enrollment = await _context.CourseEnrollments
        //        .FirstOrDefaultAsync(e => e.EnrollmentID == enrollmentID);

        //    if (enrollment == null)
        //    {
        //        return false; // Enrollment not found
        //    }

        //    // Update status to "Scheduled"
        //    enrollment.Status = "Scheduled";

        //    // Update the record in the database
        //    _context.CourseEnrollments.Update(enrollment);

        //    // Save changes
        //    var result = await _context.SaveChangesAsync();

        //    // Return true if at least one record was affected
        //    return result > 0;
        //}


       
        public async Task<IEnumerable<PrivateCourseEnrollmentViewModel>> GetPrivateEnrollmentsViewByChildAsync(int childId, String status)
        {

            return await _context.CourseEnrollments
           .Include(e => e.Course)
           .Where(e => e.ChildID == childId && e.Status == status && e.ScheduledAt == null && e.Course.CourseType == "Private") //Only root private course registrations
           .Select(e => new PrivateCourseEnrollmentViewModel
           {

               CourseID = e.CourseID,

               ChildID = e.ChildID,
               EnrollmentID = e.EnrollmentID,

               Title = e.Course.Title,
               //Address = e.Activity.Address,
               //ScheduledAt = e.Activity.ScheduledAt,
               Description = e.Course.Description,
               Status = e.Status,

               TotalCost = _context.Fees
                        .Where(f => f.CourseEnrollmentID == e.EnrollmentID)
                        .Select(f => f.TotalCost)
                        .FirstOrDefault(),
               PaymentDescription = _context.Fees
                        .Where(f => f.CourseEnrollmentID == e.EnrollmentID)
                        .Select(f => f.Description)
                        .FirstOrDefault(),
               PaymentModel = _context.Fees
                        .Where(f => f.CourseEnrollmentID == e.EnrollmentID)
                        .Select(f => f.PaymentModel)
                        .FirstOrDefault()
           })
           .OrderBy(e => e.EnrollmentID)
           .ToListAsync();



        }



        public async Task<IEnumerable<CalendarSchedule>> GetCoachSchedulesAsync(int coachId)
        {
            var sessions = await _context.CourseEnrollments
                .Where(e => e.Course.CoachID == coachId)
                .Where(e => e.ScheduledAt != null && e.Status!="Deleted" && e.Child != null)
                .Include(e => e.Course)
                .Include(e => e.Child)
                .ToListAsync();

            return sessions.Select(e => new CalendarSchedule
            {
                Title = $"{e.Child.Name}",
                Start = e.ScheduledAt.Value,
                End = e.ActualHours != null
                    ? e.ScheduledAt.Value.AddHours((double)e.ActualHours.Value)
                    : e.ScheduledAt.Value.AddHours((double)(e.ScheduledHours ?? 0)),
                Type = "Course",
                Status = e.ActualHours != null ? "Completed" : "Scheduled",
                Color = e.ActualHours != null ? "#28a745" : "#0d6efd" // 绿 / 蓝
            }).ToList();
        }




        public async Task<IEnumerable<CalendarSchedule>> UpdateCoachSchedulesAsync(int coachId)
        {
            var sessions = await _context.CourseEnrollments
                .Where(e => e.Course.CoachID == coachId)
                .Where(e => e.ScheduledAt != null)
                .Include(e => e.Course)
                .Include(e => e.Child)
                .ToListAsync();

            return sessions.Select(e => new CalendarSchedule
            {
                Title = $"{e.Child.Name}",
                Start = e.ScheduledAt.Value,
                End = e.ActualHours != null
                    ? e.ScheduledAt.Value.AddHours((double)e.ActualHours.Value)
                    : e.ScheduledAt.Value.AddHours((double)(e.ScheduledHours ?? 0)),
                Status = e.ActualHours != null ? "Completed" : "Scheduled",
                Color = e.ActualHours != null ? "#28a745" : "#0d6efd" // 绿 / 蓝
            }).ToList();
        }



        public async Task<bool> UpdateCoachSchedule(UpdateCoachScheduleViewModel vm)
        {
            var schedule = _context.CourseEnrollments.FirstOrDefault(e => e.EnrollmentID == vm.EnrollmentId);

            if (schedule == null)
                return false;
            else
            {
                //schedule.ScheduledAt = DateTime.Parse(vm.ScheduledAt);
                schedule.Location = vm.Location;
                await _context.SaveChangesAsync();
                return true;
            }
        }


        public async Task<IEnumerable<Child>> GetChildrenByCourseAsync(int courseId)
        {
            var childIds = await _context.CourseEnrollments
                .Where(e => e.CourseID == courseId && e.ChildID != null)
                .Select(e => e.ChildID.Value)
                .Distinct()
                .ToListAsync();

            return await _context.Children
                .Where(c => childIds.Contains(c.ChildID))
                .ToListAsync();



        }


        public async Task<IEnumerable<CourseEnrollmentData>> GetSessionsDataByCourseAsyn(int courseId)
        {
            return await _context.CourseEnrollments
            .Where(e => e.CourseID == courseId && e.ChildID!= null && e.ScheduledAt!= null)
            .OrderBy(e => e.ScheduledAt)
            .Select(e => new CourseEnrollmentData
            {
                ScheduledAt = e.ScheduledAt.Value,
                ChildID = e.ChildID,
                Status = e.Status
            }).ToListAsync();

        }


        //Reporting methods
        public List<StudentCourseCountDto> GetTopStudents()
        {
            return _context.CourseEnrollments
             .Where(e => e.ChildID != null
                         && (e.Status == "Completed" || e.Status == "Confirmed")
                         && e.ScheduledAt == null)
             .GroupBy(e => new { e.ChildID, e.Child.Name })
             .Select(g => new StudentCourseCountDto
             {
                 ChildId = (int)g.Key.ChildID,
                 Name = g.Key.Name,
                 Count = g.Select(x => x.CourseID).Distinct().Count()
             })
             .OrderByDescending(x => x.Count)
             .Take(10)
             .ToList();
        }

        public List<CourseDto> GetCoursesByStudent(int childId)
        {
            return _context.CourseEnrollments
                .Where(e => e.ChildID == childId)
                .Select(e => new CourseDto
                {
                    CourseName = e.Course.Title
                })
                .Distinct()
                .ToList();
        }




    }
}
