
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class CoachIncome
    {
        [Key]
        public int IncomeID { get; set; } // Primary key for the table

        // Foreign key references to the coaches, courses, and course_enrollments tables
        public int CoachID { get; set; }
        public int CourseID { get; set; }
        public int EnrollmentID { get; set; }

        // Income-related fields
        public decimal? IncomeChange { get; set; }
        public decimal? Income { get; set; }

        // Foreign keys referencing related entities
        [ForeignKey(nameof(CoachID))]
        public virtual Coach Coach { get; set; } // Navigation property for the associated Coach

        [ForeignKey(nameof(CourseID))]
        public virtual Course Course { get; set; } // Navigation property for the associated Course

        [ForeignKey(nameof(EnrollmentID))]
        public virtual CourseEnrollment Enrollment { get; set; } // Navigation property for the associated Enrollment

        // Timestamps for record creation and update
        public DateTime? CreatedDate { get; set; } 
        public int CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; } 
    }
}
