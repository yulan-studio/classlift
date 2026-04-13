
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class ActivityFeedback
    {
        [Key]
        public int FeedbackID { get; set; } // Primary key

        // Foreign key referencing the children table
        public int? ChildID { get; set; }

        [ForeignKey(nameof(ChildID))]
        public virtual Child? Child { get; set; }

        // Foreign key referencing the activities table
        public int? ActivityID { get; set; }

        [ForeignKey(nameof(ActivityID))]
        public virtual Activity? Activity { get; set; }

        // Feedback message
        [Required]
        public string Message { get; set; } = string.Empty;

        // Foreign key referencing the users table (creator of the feedback)
        [Required]
        public int CreatedBy { get; set; }

        [ForeignKey(nameof(CreatedBy))]
        public virtual User? CreatedByUser { get; set; }

        // Foreign key referencing the users table (updater of the feedback)
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
