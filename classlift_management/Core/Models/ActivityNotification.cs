
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class ActivityNotification
    {
        [Key]
        public int NotificationID { get; set; } // Primary key

        [Required]
        public string Message { get; set; } = string.Empty; // Notification message

        // Foreign key referencing the activities table
        public int? ActivityID { get; set; }

        [ForeignKey(nameof(ActivityID))]
        public virtual Activity? Activity { get; set; } // Navigation property for Activity

        // Foreign key referencing the activity_enrollments table
        public int? EnrollmentID { get; set; }

        [ForeignKey(nameof(EnrollmentID))]
        public virtual ActivityEnrollment? Enrollment { get; set; } // Navigation property for Enrollment

        // Scheduled send date/time for the notification
        public DateTime? ScheduledSend { get; set; }

        // Foreign key referencing the users table (creator of the notification)
        [Required]
        public int CreatedBy { get; set; }

        [ForeignKey(nameof(CreatedBy))]
        public virtual User? CreatedByUser { get; set; } // Navigation property for creator

        // Foreign key referencing the users table (updater of the notification)
        public int? UpdatedBy { get; set; }

        [ForeignKey(nameof(UpdatedBy))]
        public virtual User? UpdatedByUser { get; set; } // Navigation property for updater

        // Timestamps
        [Required]
        public DateTime CreatedDate { get; set; } 

        [Required]
        public DateTime UpdatedDate { get; set; }
    }
}
