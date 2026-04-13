
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Payment
    {
        [Key]
        public int PaymentID { get; set; } // Primary key for the table

        //public int UserID { get; set; }  // Foreign key for Child (nullable)
        
        public int ParentID { get; set; } // Foreign key for Parent (nullable)
        public int? PaymentPackageID { get; set; } // Foreign key for PaymentPackage (nullable)

        public int? FeeID { get; set; } // Foreign key for PaymentPackage (nullable)

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Amount { get; set; } // Amount of the payment

        [MaxLength(255)]
        public string? Receipt { get; set; } // Receipt for the payment

        public DateTime? PaymentDate { get; set; } // Date when the payment was made

        public string? Memo { get; set; } // Additional notes or memo for the payment

        public int CreatedBy { get; set; } // Foreign key for the user who created the payment
        public int? UpdatedBy { get; set; } // Foreign key for the user who updated the payment (nullable)

        // Navigation properties
        [ForeignKey(nameof(ParentID))]
        public virtual Parent? Parent { get; set; } // Navigation property for Parent

        [ForeignKey(nameof(ChildID))]
        public int ChildID { get; set; }
        public virtual Child? Child { get; set; } // Navigation property for Child


        [ForeignKey(nameof(PaymentPackageID))]
        public virtual PaymentPackage? PaymentPackage { get; set; } // Navigation property for PaymentPackage

        [ForeignKey(nameof(FeeID))]
        public virtual Fee? Fee { get; set; } // Navigation property for Fee

        [ForeignKey(nameof(CreatedBy))]
        public virtual User CreatedByUser { get; set; } // Navigation property for user who created the payment

        [ForeignKey(nameof(UpdatedBy))]
        public virtual User? UpdatedByUser { get; set; } // Navigation property for user who updated the payment (nullable)

        // Timestamps for record creation and update
        public DateTime CreatedDate { get; set; }  // Created timestamp
        public DateTime? UpdatedDate { get; set; } // Updated timestamp (nullable)
    }
}
