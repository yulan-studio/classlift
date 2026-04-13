
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class ParentChild
    {
        [Key]
        public int ParentChildID { get; set; } // Primary Key

        public int ParentID { get; set; } // Foreign Key to Parents
        public int ChildID { get; set; } // Foreign Key to Children

        public int CreatedBy { get; set; } // Created By User
        public int UpdatedBy { get; set; } // Updated By User

        public DateTime CreatedDate { get; set; } = DateTimeHelper.GetTorontoTime(); // Default Current Timestamp
        public DateTime UpdatedDate { get; set; } = DateTimeHelper.GetTorontoTime(); // Default Current Timestamp on Update

        public string Relationship { get; set; } // Relationship between Parent and Child (e.g., Father, Mother, Guardian)

        // Foreign Key relationships
        [ForeignKey("ParentID")]
        public virtual Parent Parent { get; set; } // Navigation Property to Parent

        [ForeignKey("ChildID")]
        public virtual Child Child { get; set; } // Navigation Property to Child
    }
}
