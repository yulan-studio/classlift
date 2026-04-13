using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModels
{
    public class ManageChildBalanceViewModel
    {
        public int ChildID { get; set; }

        //[Required]
        //public string ActionType { get; set; } // "Adjustment" or "Refund"

        //[Required]
        ////[Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
        //public decimal Amount { get; set; }

        public decimal CurrentBalance { get; set; }
        public IEnumerable<ChildBalance> BalanceHistory { get; set; }
    }


    public class ChildBalance
    {
        
        public DateTime CreatedDate { get; set; }
        public string Type { get; set; } // "Payment", "Course Session", "Activity"
        public string? CourseName { get; set; }
        public string? ActivityName { get; set; }

        public string? Remarks { get; set; }

        [MaxLength(255)]
        public string? Calculation { get; set; } // Receipt for the payment
        public decimal BalanceChange { get; set; }
        public decimal Balance { get; set; }

        // fields for course sessions
        public DateTime? ScheduledAt { get; set; }
        public Decimal? ActualHours { get; set; }
    }
}
