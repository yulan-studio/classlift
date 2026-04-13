
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class ChildBalance
    {
        [Key]
        public int BalanceID { get; set; } // Primary Key

        public int? ChildID { get; set; }  // Foreign Key to Children
        public int? PaymentID { get; set; } // Foreign Key to Payments
        public int? CourseID { get; set; }  // Foreign Key to Courses
        public int? ActivityID { get; set; } // Foreign Key to Activities
        public int? EnrollmentID { get; set; } // Foreign Key to CourseEnrollments

        public string? Remarks { get; set; } // Remarks or Description

        public string? Calculation { get; set; } // Remarks or Description

        required public string TransactionType { get; set; } // Transaction Type

        [Column(TypeName = "decimal(10,2)")]
        public decimal? BalanceChange { get; set; } // Balance Change Amount

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Balance { get; set; } // Current Balance

        public DateTime CreatedDate { get; set; } = DateTimeHelper.GetTorontoTime(); // Default Current Timestamp on Creation

        public int? CreatedBy { get; set; } // Created By User

        public int? UpdatedBy { get; set; } // Updated By User

        public DateTime UpdatedDate { get; set; } = DateTimeHelper.GetTorontoTime(); // Default Current Timestamp on Update

        // Foreign Key relationships
        [ForeignKey("ChildID")]
        public virtual Child Child { get; set; } // Navigation Property to Children

        [ForeignKey("PaymentID")]
        public virtual Payment Payment { get; set; } // Navigation Property to Payments

        [ForeignKey("CourseID")]
        public virtual Course Course { get; set; } // Navigation Property to Courses

        [ForeignKey("ActivityID")]
        public virtual Activity Activity { get; set; } // Navigation Property to Activities

        [ForeignKey("EnrollmentID")]
        public virtual CourseEnrollment CourseEnrollment { get; set; } // Navigation Property to CourseEnrollments
    }
}
