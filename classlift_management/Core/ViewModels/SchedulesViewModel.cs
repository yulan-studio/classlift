using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Core.Interfaces;

namespace Core.ViewModels
{

    //This is for group courses, for staff to add sessions in group course
    public class ManageSessionsViewModel
    {
        public Course Course { get; set; }

        //针对Group课程的master session
        public List<CourseEnrollment>? OpenSessions { get; set; }

        public List<CourseEnrollment>? ClosedSessions { get; set; }

        //public List<CourseEnrollment>? CanceledSessions { get; set; }

        public List<CourseEnrollment>? FinishedSessions { get; set; }

        public List<CourseEnrollment>? AllUpcomingSessions { get; set; }

		// ✅ New Property: List of Session IDs that are registered
		public List<int?> RegisteredUpcomingSessionIds { get; set; } = new List<int?>();

		public List<int> RefundedSessionEnrollmentIds { get; set; } = new();

		//针对Private课程

		//它保存 Private 课程的学生注册记录
		public List<CourseEnrollment> PrivateRegistrations { get; set; } = new();

		// Scheduled child sessions for Staff-managed Private courses.
		public List<CourseEnrollment> PrivateSessions { get; set; } = new();

		// Null means unlimited; otherwise this is the remaining count for the registration.
		public Dictionary<int, int?> PrivateRegistrationRemainingSessions { get; set; } = new();

       
        

        
    }

    public sealed class ManageGroupSessionsViewModel
    {
        public Course Course { get; set; }
        public List<CourseEnrollment>? OpenSessions { get; set; }
        public List<CourseEnrollment>? ClosedSessions { get; set; }
        public List<CourseEnrollment>? FinishedSessions { get; set; }
        public List<CourseEnrollment>? AllUpcomingSessions { get; set; }
        public List<int?> RegisteredUpcomingSessionIds { get; set; } = new();
        public List<int> RefundedSessionEnrollmentIds { get; set; } = new();
    }

    public sealed class ManagePrivateSessionsViewModel
    {
        public Course Course { get; set; }
        public List<CourseEnrollment> PrivateRegistrations { get; set; } = new();
        public List<CourseEnrollment> PrivateSessions { get; set; } = new();
        public Dictionary<int, int?> PrivateRegistrationRemainingSessions { get; set; } = new();
    }
}
