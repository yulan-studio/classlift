using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModels
{
    public class PrivateCourseEnrollmentViewModel  //This is show course enrollment information for all registered courses for a child
    {
        public int? ChildID { get; set; }
        public int CourseID { get; set; }

        //public bool IsActive { get; set; }

        public bool IsPaid { get; set; }

        public decimal? TotalCost { get; set; }

        public required string PaymentModel { get; set; }

        public string? PaymentDescription { get; set; }

        public int EnrollmentID { get; set; }  //this is the EnrollmentID for each course which status is 'registered' for the child
        public required string Title { get; set; }

        //public required string Location { get; set; }

        public string? Description { get; set; }

        //public DateTime ScheduledAt { get; set; }
        public required string Status { get; set; }
 
    }
}
