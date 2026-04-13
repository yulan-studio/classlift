using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Activity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ActivityID { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        public string? Description { get; set; }

        [MaxLength(255)]
        public string Address { get; set; }

        public int MaxCapacity { get; set; }

        public DateTime ScheduledAt { get; set; }

        public decimal? ScheduledHours { get; set; }

        //[Column(TypeName = "decimal(10, 2)")]
        //public decimal Cost { get; set; }

        //public bool IsActive { get; set; } = true;

        public string Status { get; set; } 

        public int? ContactID { get; set; }

        [Required]
        public int CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } 

        public DateTime UpdatedDate { get; set; } 

        // Navigation properties for foreign key relationships
        [ForeignKey(nameof(ContactID))]
        public virtual User? Contact { get; set; }

        [ForeignKey(nameof(CreatedBy))]
        public virtual User CreatedByUser { get; set; }

        [ForeignKey(nameof(UpdatedBy))]
        public virtual User? UpdatedByUser { get; set; }
    }
}
