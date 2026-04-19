using Core.DTOs;
using Core.Interfaces;
using Core.Models;
using Core.Repositories;
using Core.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public class CourseEnrollmentService : ICourseEnrollmentService
    {
        private readonly ICourseEnrollmentRepository _enrollmentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IChildRepository _childRepository;
        private readonly ICoachRepository _coachRepository;

        public CourseEnrollmentService(ICourseEnrollmentRepository enrollmentRepository, ICourseRepository courseRepository, IChildRepository childRepository, ICoachRepository coachRepository)
        {
            _enrollmentRepository = enrollmentRepository;
            _courseRepository = courseRepository;
            _childRepository = childRepository;
            _coachRepository = coachRepository;
        }

        // Get a enrollment by ID
        public async Task<CourseEnrollment> GetAsync(int enrollmentId)
        {
            var enrollment = await _enrollmentRepository.GetAsync(enrollmentId);
            if (enrollment == null)
            {
                throw new KeyNotFoundException($"Enrollment with ID {enrollmentId} not found.");
            }
            return enrollment;
        }

      


        public async Task<bool> RemoveAsync(int enrollmentId)
        {
            try 
            {
                return await _enrollmentRepository.RemoveAsync(enrollmentId);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
            
            
        }


        

       



        public async Task<bool> IsChildEnrolledInCourse(int childId, int courseId)
        {
            var enrollments1 = await _enrollmentRepository.GetRootEnrollmentsByChildAsync(childId, "Registered");
            var enrollments2 = await _enrollmentRepository.GetRootEnrollmentsByChildAsync(childId, "Confirmed");
            return enrollments1.Any(e => e.CourseID == courseId) || enrollments2.Any(e => e.CourseID == courseId);
        }


        public async Task<bool> IsChildEnrolledInCourseSession(int childId, int enrollmentId_Ref)
        {
            var enrollments = await _enrollmentRepository.GetEnrollmentsByChildAsync(childId);
            return enrollments.Any(e => e.EnrollmentID_Ref == enrollmentId_Ref && e.Status != "Deleted");
        }



        //Register course to child
        public async Task<int> AddRegisteredEnrollmentAsync(int childId, int courseId, decimal scheduledHours, string status, User user)
        {
            if (childId <= 0 || courseId <= 0 || scheduledHours < 0)
                throw new ArgumentException("Invalid child, course, or hours.");

            // ✅ Retrieve Course and Child from the database
            var child = await _childRepository.GetAsync(childId);
            var course = await _courseRepository.GetAsync(courseId);

            if (child == null || course == null)
                throw new ArgumentException("Invalid child or course.");




            if(await IsChildEnrolledInCourse(child.ChildID, courseId))
                throw new ArgumentException("This course has already been registered.");


            try
            {
                var enrollment = new CourseEnrollment
                {
                    //ChildID = childId,
                    CourseID = courseId,
                    ScheduledHours = scheduledHours,
                    Child = child,
                    Course = course,
                    CreatedBy = user.Id,
                    CreatedDate = DateTimeHelper.GetTorontoTime(),
                    Status = status
                };

                await _enrollmentRepository.AddAsync(enrollment);
                return enrollment.EnrollmentID;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
           
        }
       
        //Unregister course to child
        public async Task<bool> RemoveRegisteredEnrollmentAsync(int enrollmentId)
        {
            //Enrollment removal is only allowed for courses that have not started.
            var enrollment = await _enrollmentRepository.GetAsync(enrollmentId);
            var childId = enrollment.ChildID;
            var courseId = enrollment.CourseID;
            var course_enrollment = await _enrollmentRepository.GetEnrollmentsByCourseAsync(courseId);
            if (course_enrollment.Any())
            {
                foreach (var e in course_enrollment)
                {
                    if (e.ChildID == childId && (e.Status == "Scheduled" || e.Status == "Completed" || e.Status =="RequestToReschedule" || e.Status == "RequestToLeave" || e.Status =="OnLeave" ))
                        throw new Exception("This registration cannot be removed because the child has scheduled sessions in this course. Please cancel all sessions before removing the registration.");
                }

                foreach (var e in course_enrollment)  // Remove all scheduled sessions for the child in this course that has not been confirmed yet
                {
                    if (e.ChildID == childId && (e.Status == "Registered"||e.Status =="Canceled"))
                        await _enrollmentRepository.RemoveAsync(e.EnrollmentID);
                }

            }
            return await _enrollmentRepository.RemoveAsync(enrollmentId);
        }



        //Register grouop course session to child

        public async Task<bool> AddSessionRegisteredEnrollmentAsync(int childId, int courseId, DateTime? scheduledAt, decimal? scheduledHours, string? location, int enrollmentId_Ref, string status, User user)
        {
            //if (userId <= 0 || courseId <= 0 || scheduledHours < 0)
            //    throw new ArgumentException("Invalid child, course, or hours.");

            // ✅ Retrieve Course and Child from the database
            var child = await _childRepository.GetAsync(childId);
            var course = await _courseRepository.GetAsync(courseId);

            if (child == null || course == null)
                throw new ArgumentException("Invalid child or course.");


            if (await IsChildEnrolledInCourseSession(child.ChildID, enrollmentId_Ref))
                throw new ArgumentException("This course session has already been registered.");


            try
            {
                var enrollment = new CourseEnrollment
                {
                    //ChildID = childId,
                    CourseID = courseId,
                    ScheduledHours = scheduledHours,
                    Child = child,
                    Course = course,
                    ScheduledAt = scheduledAt,
                    Location = location,
                    EnrollmentID_Ref = enrollmentId_Ref,
                    CreatedBy = user.Id,
                    CreatedDate = DateTimeHelper.GetTorontoTime(),
                    Status = status
                };

                return await _enrollmentRepository.AddAsync(enrollment);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

        }




        //

        //public async Task<IEnumerable<CourseEnrollment>> GetRegisteredEnrollmentsByChildAsync(int childId)
        //{
        //    return await _enrollmentRepository.GetEnrollmentsByChildAsync(childId, "Registered");
        //}

        //Get Upcoming course schedules
        public async Task<IEnumerable<CourseEnrollment>> GetUpcomingEnrollmentsByChildAsync(int childId)
        {
            return await _enrollmentRepository.GetUpcomingEnrollmentsByChildAsync(childId);
        }



        public async Task<IEnumerable<CourseEnrollmentViewModel>> GetRegisteredEnrollmentsByChildAsync(int childId)
        {
            return await _enrollmentRepository.GetRegisteredEnrollmentsByChildAsync(childId);
        }



        public async Task<IEnumerable<CourseEnrollment>> GetFinishedEnrollmentsByChildAsync(int childId)
        {
            return await _enrollmentRepository.GetFinishedEnrollmentsByChildAsync(childId);
        }

        //public async Task<IEnumerable<Child>> GetRegisteredChildrenByCoachAsync(int coachId)
        //{
        //    var course_enrollments =  await _enrollmentRepository.GetEnrollmentsByCoachAsync(coachId, "Registered");

        //    return course_enrollments.Select(e => e.Child).ToList();

        //}

        public async Task<IEnumerable<CourseEnrollment>> GetScheduledEnrollmentsByCourseAsync(int courseId)
        {
            return await _enrollmentRepository.GetEnrollmentsByCourseAsync(courseId, "Scheduled");
        }

        public async Task<IEnumerable<CourseEnrollment>> GetRegisteredEnrollmentsByCourseAsync(int courseId)
        {
            return await _enrollmentRepository.GetEnrollmentsByCourseAsync(courseId, "Registered");
        }

        //This is for Group Course (When Status is set to 'Open', means it is open for registration)
        public async Task<IEnumerable<CourseEnrollment>> GetOpenSessionsByCourseAsync(int courseId)
        {
           return await _enrollmentRepository.GetSessionsByCourseAsync(courseId, "Open");
        }

        //This is for Group Course (When Status is set to 'Closed', means it is closed for registration)
        public async Task<IEnumerable<CourseEnrollment>> GetClosedSessionsByCourseAsync(int courseId)
        {
            return await _enrollmentRepository.GetSessionsByCourseAsync(courseId, "Closed");
        }

        //This is for Group Course (When Status is set to 'Canceled', means it is canceled for unexpected reason)
        public async Task<IEnumerable<CourseEnrollment>> GetCanceledSessionsByCourseAsync(int courseId)
        {
            return await _enrollmentRepository.GetSessionsByCourseAsync(courseId, "Canceled");
        }

        //This is for Group Course (When Status is set to 'Completed', means it is completed)
        public async Task<IEnumerable<CourseEnrollment>> GetCompletedSessionsByCourseAsync(int courseId)
        {
            return await _enrollmentRepository.GetSessionsByCourseAsync(courseId, "Completed");
        }

        public async Task<IEnumerable<CourseEnrollment>> GetAllPastSessionsByCourseAsync(int courseId)
        {
            return await _enrollmentRepository.GetAllPastSessionsByCourseAsync(courseId);
        }

        public async Task<IEnumerable<CourseEnrollment>> GetAllUpcomingSessionsByCourseAsync(int courseId)
        {
            return await _enrollmentRepository.GetAllUpcomingSessionsByCourseAsync(courseId);
        }

        public async Task<List<int?>> GetRegisteredUpcomingSessionsByCourseAsync(int courseId)
        {
            return await _enrollmentRepository.GetRegisteredUpcomingSessionsByCourseAsync(courseId);
        }

        //Get Registered children for a course
        public async Task<IEnumerable<Core.ViewModels.ChildViewModel>> GetRegisterationByCourseAsync(int courseId)
        {
            //var course_enrollments = await _enrollmentRepository.GetEnrollmentsByCourseAsync(courseId, "Registered");



            //var childTasks = course_enrollments.Select(async e => new ChildViewModel
            //{
            //    EnrollmentID = e.EnrollmentID,
            //    ChildID = e.Child.ChildID,
            //    Name = e.Child.Name,
            //    Gender = e.Child.Gender,
            //    City = e.Child.City.Name,
            //    BirthDate = e.Child.BirthDate,
            //    RegisteredDate = e.CreatedDate, // Ensure CreatedDate maps to RegisteredDate
            //    Scheduled = (await _enrollmentRepository.GetEnrollmentsByCourseChildAsync(e.CourseID, (int)e.ChildID, "Scheduled")).Count(),
            //    RequestToReschedule = (await _enrollmentRepository.GetEnrollmentsByCourseChildAsync(e.CourseID, (int)e.ChildID, "RequestToReschedule")).Count(),
            //    Completed = (await _enrollmentRepository.GetEnrollmentsByCourseChildAsync(e.CourseID, (int)e.ChildID, "Completed")).Count()
            //}).ToList();

            //var children = (await Task.WhenAll(childTasks)).ToList();

            //return children;

            var course_enrollments = await _enrollmentRepository.GetEnrollmentsByCourseAsync(courseId, "Confirmed");

            var children = new List<ChildViewModel>();

            foreach (var e in course_enrollments)
            {
                var scheduled = await _enrollmentRepository.GetEnrollmentsByCourseChildAsync(e.CourseID, (int)e.ChildID, "Scheduled");
                var requestToReschedule = await _enrollmentRepository.GetEnrollmentsByCourseChildAsync(e.CourseID, (int)e.ChildID, "RequestToReschedule");
                var completed = await _enrollmentRepository.GetEnrollmentsByCourseChildAsync(e.CourseID, (int)e.ChildID, "Completed");
                var deleted = await _enrollmentRepository.GetEnrollmentsByCourseChildAsync(e.CourseID, (int)e.ChildID, "Deleted");

                children.Add(new ChildViewModel
                {
                    EnrollmentID = e.EnrollmentID,
                    ChildID = e.Child.ChildID,
                    Name = e.Child.Name,
                    Gender = e.Child.Gender,
                    City = e.Child.City.Name,
                    BirthDate = e.Child.BirthDate,
                    RegisteredDate = e.CreatedDate,
                    Scheduled = scheduled.Count(),
                    RequestToReschedule = requestToReschedule.Count(),
                    Deleted = deleted.Count(),
                    Completed = completed.Count()
                });
            }

            return children;

        }

        //Schedule Private Course for a child
        public async Task<bool> ScheduleCourseAsync(int childId, int courseId, DateTime scheduledAt, decimal scheduledHours, string location, int coachId, int enrollmentId_Ref)
        {
            Child? child = await _childRepository.GetAsync(childId);
            if (child == null)
                throw new ArgumentException("Invalid child.");
            Coach? coach = await _coachRepository.GetAsync(coachId);
            if(coach == null)
                throw new ArgumentException("Invalid coach.");
            Course course = await _courseRepository.GetAsync(courseId);
            var enrollment = new CourseEnrollment
            {
                //UserID = userId,
                Child = child,  
                Course = course,
                ScheduledAt = scheduledAt,
                ScheduledHours = scheduledHours,
                Location = location,
                CreatedBy = coach.UserID,
                CreatedDate = DateTimeHelper.GetTorontoTime(),
                Status = "Scheduled",
                EnrollmentID_Ref = enrollmentId_Ref
            };

            try
            {
                return await _enrollmentRepository.AddAsync(enrollment);
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }


        //Add new session to Group Course
        public async Task<bool> AddSessionToGroupCourseAsync(int courseId, DateTime scheduledAt, decimal scheduledHours, string location, string staffNote, User user)
        {
          
            Course course = await _courseRepository.GetAsync(courseId);
            var newSession = new CourseEnrollment
            {
                
                ChildID = null,
                Course = course,
                ScheduledAt = scheduledAt,
                ScheduledHours = scheduledHours,
                Location = location,
                StaffNote = staffNote,
                CreatedBy = user.Id,
                CreatedDate = DateTimeHelper.GetTorontoTime(),
                Status = "Open"
            };

            try
            {
                return await _enrollmentRepository.AddAsync(newSession);
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }



        public async Task<bool> UpdateSessionAsync(CourseEnrollment session)
        {
            var existingSession = await _enrollmentRepository.GetAsync(session.EnrollmentID);
            if (existingSession == null)
                throw new KeyNotFoundException("Session not found.");

            existingSession.Status = session.Status;
            existingSession.StaffNote = session.StaffNote;
            existingSession.ParentNote = session.ParentNote;
            existingSession.Location = session.Location;

            return await _enrollmentRepository.UpdateAsync(existingSession);
        }


        //Set Session status of children registration to be completed 
        public async Task UpdateChildCompletedSessionsAsync(int courseId)
        {
            await _enrollmentRepository.UpdateChildCompletedSessionsAsync(courseId);
        }

        //Set Session status to be completed after the session past the scheduled time for group course
        public async Task UpdateCompletedSessionsAsync(int courseId)
        {
            await _enrollmentRepository.UpdateCompletedSessionsAsync(courseId);
        }

        //Set Session status of children registration to be canceled 
        public async Task UpdateChildCanceledSessionsAsync(int enrollmentId, string staffNote)
        {
            await _enrollmentRepository.UpdateChildCanceledSessionsAsync(enrollmentId, staffNote);
        }


        //This is set Status to "Deleted", not actually delete the record. All the Delete change made by Coach can be seen by Coach and Parent.
        public async Task<bool> RemoveScheduleAsync(int enrollmentId, string coachNote)
        {
            //Enrollment removal is only allowed for courses that have not started.
            var enrollment = await _enrollmentRepository.GetAsync(enrollmentId);
            if (enrollment == null)
                throw new ArgumentException("Invalid scheduled course.");

            if (enrollment.Status == "Scheduled" || enrollment.Status == "RequestToReschedule")
            {
                //throw new ArgumentException("This is not scheduled");
                enrollment.Status = "Deleted";
                enrollment.CoachNote = coachNote;
            }

            if (coachNote == "MISTAKE")
            {
                try
                {
                    //return await _enrollmentRepository.RemoveAsync(enrollmentId);
                    return await _enrollmentRepository.DeleteAsync(enrollment);
                }

                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex.InnerException);
                }

            }

            else 
            { 
                try
                {
                    //return await _enrollmentRepository.RemoveAsync(enrollmentId);
                    return await _enrollmentRepository.UpdateAsync(enrollment);
                }

                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex.InnerException);
                }
            }
        }

        //Get Scheduled Sessions (Include Group Course Session and Private Course Sessions)
        public async Task<IEnumerable<CourseEnrollment>> GetScheduledSessionsByChildAsync(int childId)
        {
          
            Child? child = await _childRepository.GetAsync(childId);
            if (child == null)
                throw new ArgumentException("Invalid child.");

            return await _enrollmentRepository.GetEnrollmentsByChildAsync(childId, "Scheduled");
        }


        //Get Registered Sessions (For Group Course Sessions), these sessions need to be confirmed by parent to change status to 'Scheduled'
        public async Task<IEnumerable<CourseEnrollment>> GetScheduledSessionsToConfirmByChildAsync(int childId)
        {

            Child? child = await _childRepository.GetAsync(childId);
            if (child == null)
                throw new ArgumentException("Invalid child.");

            return await _enrollmentRepository.GetScheduledSessionsToConfirmByChildAsync(childId);
        }

        public async Task<IEnumerable<PrivateCourseEnrollmentViewModel>> GetPrivateEnrollmentsViewByChildAsync(int childId, String status)
        {
            return await _enrollmentRepository.GetPrivateEnrollmentsViewByChildAsync(childId, status);
        }


        public async Task<IEnumerable<CourseEnrollment>> GetRegisteredByCourseChildAsync(int courseId, int childId)
        {
            Child? child = await _childRepository.GetAsync(childId);
            if (child == null)
                throw new ArgumentException("Invalid child.");
            return await _enrollmentRepository.GetEnrollmentsByCourseChildAsync(courseId, childId, "Registered");
        }


        public async Task<IEnumerable<CourseEnrollment>> GetSchedulesByCourseChildAsync(int courseId, int childId)
        {
            Child? child = await _childRepository.GetAsync(childId);
            if (child == null)
                throw new ArgumentException("Invalid child.");
            return await _enrollmentRepository.GetEnrollmentsByCourseChildAsync(courseId, childId, "Scheduled");
        }


        public async Task<IEnumerable<CourseEnrollment>> GetWaitToCompleteByCourseChildAsync(int courseId, int childId)
        {
            Child? child = await _childRepository.GetAsync(childId);
            if (child == null)
                throw new ArgumentException("Invalid child.");
            //return await _enrollmentRepository.GetEnrollmentsByCourseChildAsync(courseId, childId, "Scheduled");
            return await _enrollmentRepository.GetOverduedEnrollmentsByCourseChildAsync(courseId, childId, "Scheduled");
        }

        public async Task<IEnumerable<CourseEnrollment>> GetCompletesByCourseChildAsync(int courseId, int childId)
        {
            Child? child = await _childRepository.GetAsync(childId);
            if (child == null)
                throw new ArgumentException("Invalid child.");
            return await _enrollmentRepository.GetEnrollmentsByCourseChildAsync(courseId, childId, "Completed");
        }

        public async Task<IEnumerable<CourseEnrollment>> GetDeletedByCourseChildAsync(int courseId, int childId)
        {
            Child? child = await _childRepository.GetAsync(childId);
            if (child == null)
                throw new ArgumentException("Invalid child.");
            return await _enrollmentRepository.GetEnrollmentsByCourseChildAsync(courseId, childId, "Deleted");
        }


        public async Task<IEnumerable<CourseEnrollment>> GetEnrollmentsByCourseChildAsync(int courseId, int childId)
        {
            Child? child = await _childRepository.GetAsync(childId);
            if (child == null)
                throw new ArgumentException("Invalid child.");
            return await _enrollmentRepository.GetEnrollmentsByCourseChildAsync(courseId, childId);

        }


        public async Task<IEnumerable<CourseEnrollment>> GetEnrollments2ByCourseChildAsync(int courseId, int childId)
        {
            Child? child = await _childRepository.GetAsync(childId);
            if (child == null)
                throw new ArgumentException("Invalid child.");
            return await _enrollmentRepository.GetEnrollments2ByCourseChildAsync(courseId, childId);

        }


        public async Task<int?> GetEnrollmentIdByChildAndCourseAsync(int courseId, int childId, string status)
        {
            
            return await _enrollmentRepository.GetEnrollmentIdByChildAndCourseAsync(courseId, childId, status);

        }


        public async Task<IEnumerable<CourseEnrollment>> GetUpcomingEnrollmentsByCourseChildAsync(int courseId, int childId)
        {
            Child? child = await _childRepository.GetAsync(childId);
            if (child == null)
                throw new ArgumentException("Invalid child.");
            return await _enrollmentRepository.GetUpcomingEnrollmentsByCourseChildAsync(courseId, childId);

        }


        public async Task<bool> CompleteSessionAsync(int enrollmentId, decimal actualHours, string coachNote)
        {
            //Enrollment removal is only allowed for courses that have not started.
            var enrollment = await _enrollmentRepository.GetAsync(enrollmentId);
            if (enrollment == null)
                throw new ArgumentException("Invalid scheduled session.");

            if (enrollment.Status != "Scheduled")
                //throw new ArgumentException("This is not scheduled");
                return false;

            if (actualHours < 0)
            {
                throw new Exception("Actual Hours must be equal or greater than zero.");
            }

            enrollment.Status = "Completed";
            enrollment.ActualHours = actualHours;
            enrollment.CoachNote = coachNote;

            if(enrollment.ScheduledAt.HasValue && enrollment.ScheduledHours.HasValue && DateTime.UtcNow < enrollment.ScheduledAt.Value.AddHours((double)enrollment.ScheduledHours))
            {
                throw new Exception("You’ll be able to complete the session only after the scheduled date has passed.");
            }
            enrollment.UpdatedDate = DateTimeHelper.GetTorontoTime();

            try
            {
                return await _enrollmentRepository.UpdateAsync(enrollment);
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }


        //Update course to be completed for private course
        public async Task<bool> UpdateCompletedCoursesAsync()
        {
            return await _enrollmentRepository.UpdateCompletedCoursesAsync();
        }


        
        public async Task<List<int?>> GetChildrenWithRequestsOrConcernsAsync()
        {
            return await _enrollmentRepository.GetChildrenWithRequestsOrConcernsAsync();
        }

        public async Task<List<int?>> GetChildrenWithConcernsAsync()
        {
            return await _enrollmentRepository.GetChildrenWithScheduleConcernsAsync();
        }

        public async Task<List<int>> GeEnrollmentsWithScheduleConcernsAsync()
        {
            return await _enrollmentRepository.GetEnrollmentsWithScheduleConcernsAsync();
        }


        //public async Task<bool> UpdateCourseStatusToConfirmedAsync(int courseId)
        //{

        //    return await _enrollmentRepository.UpdateCourseStatusToScheduledAsync(courseId);

        //}


        public async Task<bool> UpdateCourseEnrollmentStatusToConfirmedAsync(int enrollmentId)
        {

            return await _enrollmentRepository.UpdateCourseEnrollmentStatusToConfirmedAsync(enrollmentId);

        }


        public async Task<IEnumerable<CalendarSchedule>> GetCoachSchedulesAsync(int coachId)
        {
            return await _enrollmentRepository.GetCoachSchedulesAsync(coachId);
        }


        public async Task<bool> UpdateCoachSchedule(UpdateCoachScheduleViewModel vm)
        {
            return await _enrollmentRepository.UpdateCoachSchedule(vm);
        }

        //public async Task<IEnumerable<Child>> GetChildrenByCourseAsync(int courseId)
        //{
        //    return await _enrollmentRepository.GetChildrenByCourseAsync(courseId);
        //}

        //public async Task<IEnumerable<CourseEnrollmentData>> GetSessionsByCourseAsyn(int courseId)
        //{
        //    return await _enrollmentRepository.GetSessionsByCourseAsyn(courseId);
        //}

        public async Task<SessionAttendanceViewModel> GetAttendanceAsync(int courseId)
        {
            var course = await _courseRepository.GetAsync(courseId);
            var children = await _enrollmentRepository.GetChildrenByCourseAsync(courseId);
            var sessionsData = await _enrollmentRepository.GetSessionsDataByCourseAsyn(courseId);

            var vm = new SessionAttendanceViewModel();

            vm.Course = course;

            foreach (var c in children)
            {
                vm.ChildrenList.Add(new ChildInfo
                {
                    ChildID = c.ChildID,
                    Name = c.Name
                });
            }

            var sessionsGroup = sessionsData.GroupBy(s => s.ScheduledAt);

            foreach (var session in sessionsGroup)
            {
                var sessionInfo = new SessionInfo
                {
                    ScheduledAt = session.Key
                };

                foreach (var s in session)
                {
                    if (s.ChildID.HasValue)
                        sessionInfo.Children[s.ChildID.Value] = s.Status;
                }

                vm.Sessions.Add(sessionInfo);
            }

            return vm;
        }
    


}


}





