
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class PaymentPackage
    {
        [Key]
        public int PackageID { get; set; } // Primary Key

        [Required]
        [StringLength(255)]
        public string Title { get; set; } // Package Title

        public string Description { get; set; } // Package Description

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? Amount { get; set; } // Package Amount

        public bool IsActive { get; set; } = true; // Active status (default is 1)

        public int CreatedBy { get; set; } // Created By User

        public int? UpdatedBy { get; set; } // Updated By User (nullable)

        public DateTime CreatedDate { get; set; } = DateTimeHelper.GetTorontoTime(); // Default Current Timestamp

        public DateTime UpdatedDate { get; set; } = DateTimeHelper.GetTorontoTime(); // Default Current Timestamp on Update
    }
}
