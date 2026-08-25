using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class Province
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProvinceID { get; set; }

        [Required]
        [StringLength(255)]
        public required string Name { get; set; }

        [ForeignKey(nameof(CreatedByUser))]
        public int CreatedBy { get; set; }

        [ForeignKey(nameof(UpdatedByUser))]
        public int? UpdatedBy { get; set; }

        [Column(TypeName = "timestamp")]
        public DateTime? CreatedDate { get; set; }

        [Column(TypeName = "timestamp")]
        public DateTime? UpdatedDate { get; set; }

        public virtual User? CreatedByUser { get; set; }
        public virtual User? UpdatedByUser { get; set; }
        public virtual ICollection<City> Cities { get; set; } = new List<City>();
    }
}
