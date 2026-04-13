using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class ActivityEnrollment
    {
        [Key]
        public int EnrollmentID { get; set; } // Primary key

        // Foreign key referencing the children table
        public int ChildID { get; set; }

        [ForeignKey(nameof(ChildID))]
        public virtual Child Child { get; set; }

        // Foreign key referencing the activities table
        public int ActivityID { get; set; }

        [ForeignKey(nameof(ActivityID))]
        public virtual Activity Activity { get; set; }

        // Status of the enrollment (e.g., Pending, Approved, Cancelled)
        [StringLength(50)]
        public string Status { get; set; }

        // Foreign key referencing the users table (creator of the record)
        [Required]
        public int CreatedBy { get; set; }

        [ForeignKey(nameof(CreatedBy))]
        public virtual User? CreatedByUser { get; set; }

        // Foreign key referencing the users table (updater of the record)
        public int? UpdatedBy { get; set; }

        [ForeignKey(nameof(UpdatedBy))]
        public virtual User? UpdatedByUser { get; set; }

        // Timestamps
        [Required]
        public DateTime CreatedDate { get; set; } 

        [Required]
        public DateTime UpdatedDate { get; set; } 
    }
}
