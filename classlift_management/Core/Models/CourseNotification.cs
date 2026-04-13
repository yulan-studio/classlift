
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class CourseNotification
    {
        [Key]
        public int NotificationID { get; set; }        // NotificationID (Primary Key)
        public string Message { get; set; }            // Message
        public int? CourseID { get; set; }             // CourseID (Foreign Key, Nullable)
        public int? EnrollmentID { get; set; }         // EnrollmentID (Foreign Key, Nullable)
        public int CreatedBy { get; set; }             // CreatedBy (Foreign Key)
        public int? UpdatedBy { get; set; }            // UpdatedBy (Foreign Key, Nullable)
        public DateTime CreatedDate { get; set; }      // CreatedDate
        public DateTime UpdatedDate { get; set; }      // UpdatedDate

        // Navigation Properties (Optional, you can add these if you plan to use Entity Framework to establish relationships)

        // For Course table (Foreign Key)
        public Course Course { get; set; }

        // For CourseEnrollment table (Foreign Key)
        public CourseEnrollment Enrollment { get; set; }

        // For User table (Foreign Key - CreatedBy)
        public User CreatedByUser { get; set; }

        // For User table (Foreign Key - UpdatedBy)
        public User UpdatedByUser { get; set; }
    }
}