

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Fee
    {
        public int FeeID { get; set; }

        public int? CourseEnrollmentID { get; set; }

        public int? ActivityEnrollmentID { get; set; }

        public required string? Description { get; set; }

        public required decimal? TotalCost { get; set; }

        public required string PaymentModel { get; set; }

        public bool IsPaid { get; set; }

        public DateTime? PaidAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public int? CreatedBy { get; set; }
        [ForeignKey("CreatedBy")]
        public virtual User CreatedByUser { get; set; } // Navigation property to User (CreatedBy)

        public DateTime UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        [ForeignKey("UpdatedBy")]
        public virtual User UpdatedByUser { get; set; } // Navigation property to User (UpdatedBy)

        

        // Navigation properties
        public virtual CourseEnrollment? CourseEnrollment { get; set; }

        public virtual ActivityEnrollment? ActivityEnrollment { get; set; }


    }
}
