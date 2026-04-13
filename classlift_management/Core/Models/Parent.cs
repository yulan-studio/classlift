
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Parent
    {
        [Key]
        public int ParentID { get; set; } // Primary Key

        [Required]
        [StringLength(255)]
        public string Name { get; set; } // Parent Name

        [StringLength(50)]
        public string? Phone { get; set; } // Phone Number (nullable)

        [StringLength(255)]
        public string? Email { get; set; } // Email (nullable)

        [StringLength(100)]
        public string? Wechat { get; set; } // WeChat ID (nullable)

        //[Required]
        //public string Gender { get; set; }

        public int? CreatedBy { get; set; } // Created By User ID (nullable)

        public int? UpdatedBy { get; set; } // Updated By User ID (nullable)

        public DateTime? CreatedDate { get; set; } = DateTimeHelper.GetTorontoTime(); // Created Timestamp (nullable)

        public DateTime? UpdatedDate { get; set; } = DateTimeHelper.GetTorontoTime(); // Updated Timestamp (nullable)

        // Navigation properties
        [ForeignKey("CreatedBy")]
        public virtual User CreatedByUser { get; set; }

        [ForeignKey("UpdatedBy")]
        public virtual User UpdatedByUser { get; set; }

        // ✅ New: List of related children
        public virtual ICollection<ParentChild> ParentChild { get; set; } = new List<ParentChild>();
    }


   
}
