

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Specialty
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SpecialtyID { get; set; }

        [Required]
        [StringLength(255)]
        public required string Title { get; set; }

        [Required]
        public required string Description { get; set; }

        //[Required]
        public ICollection<CoachSpecialty>? CoachSpecialties { get; set; }

        [Required]
        [ForeignKey("CreatedByUser")]
        public int CreatedBy { get; set; }

        [ForeignKey("UpdatedByUser")]
        public int? UpdatedBy { get; set; }

        [Column(TypeName = "timestamp")]
        public DateTime? CreatedDate { get; set; } 

        [Column(TypeName = "timestamp")]
        public DateTime? UpdatedDate { get; set; } 

        // Navigation properties
        public virtual User? CreatedByUser { get; set; }
        public virtual User? UpdatedByUser { get; set; }


    }
}
