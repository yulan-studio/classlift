using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModels
{
    public class CourseEnrollmentViewModel  //This is show course enrollment information for all registered courses for a child
    {
        public int? ChildID { get; set; }
        public int CourseID { get; set; }

        public string CourseType { get; set; }

        public bool IsActive { get; set; }

        public bool IsPaid { get; set; }

        public decimal? TotalCost { get; set; }

        public required string PaymentModel { get; set; }

        public string? PaymentDescription { get; set; }

        public int EnrollmentID { get; set; }  //this is the EnrollmentID for each course which status is 'registered' for the child
        public string Title { get; set; }

        public int? SessionCount { get; set; }
        public string CoachName { get; set; }
        public string SpecialtyName { get; set; }

        public decimal? HourlyCost { get; set; }

        public decimal? HourlyCost2 { get; set; }
        public string Status { get; set; }

        public int RegisteredSessions { get; set; }
        public int ScheduledSessions { get; set; }
        public int CompletedSessions { get; set; }

        public int CanceledSessions { get; set; }
        public int RequestToLeaveSessions { get; set; }

        public int RequestToRescheduleSessions { get; set; }

        public int OnLeaveSessions { get; set; }
    }
}
