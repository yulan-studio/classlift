
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Coach
    {



        //[Required]
        //public int UserID { get; set; }
        //[ForeignKey("UserID")]
        //public virtual User User { get; set; } // Navigation property to User table (UserID)
        public int CoachID { get; set; } // Primary key

        public int UserID { get; set; }

        public virtual User User { get; set; }

        [MaxLength(255)]
        public required string Name { get; set; }

        public string? PreferedName { get; set; }

        public string? MemberID { get; set; }

        [MaxLength(255)]
        public required string Gender { get; set; }

        //public int SpecialtyID { get; set; }
        //[ForeignKey("SpecialtyID")]
        //public virtual required Specialty Specialty { get; set; } // Navigation property to Speical table (SpecialID)

        [MaxLength(50)]
        public string? Phone { get; set; }

        //[MaxLength(255)]
        //public string Email { get; set; }

        [MaxLength(100)]
        public string? Wechat { get; set; }

        [MaxLength(100)]
        public string? Address { get; set; }

        [MaxLength(6)]
        public string? PostCode { get; set; }

        public int CityID { get; set; }
        [ForeignKey("CityID")]
        public virtual required City City { get; set; } // Navigation property to Speical table (SpecialID)

        [Range(000, 999, ErrorMessage = "Bank must be a 3-digit number.")]
        public int? Bank { get; set; }


        [Range(00000, 99999, ErrorMessage = "Transit must be a 5-digit number.")]
        public int? Transit { get; set; }


        [Range(00000, 99999, ErrorMessage = "Account must be a 5-digit number.")]
        public int? Account { get; set; }

        public string? Status { get; set; }

        public bool PhotoConsent { get; set; }

        // Many-to-Many Relationship
        [Required]
        public ICollection<CoachSpecialty> CoachSpecialties { get; set; }


        public ICollection<EmergencyContact>? EmergencyContacts { get; set; } = new List<EmergencyContact>();

        //public List<Specialty>? Specialties { get; set; }


        //public int? CreatedBy { get; set; }
        //[ForeignKey("CreatedBy")]
        //public virtual User CreatedByUser { get; set; } // Navigation property to User (CreatedBy)

        //public int? UpdatedBy { get; set; }
        //[ForeignKey("UpdatedBy")]
        //public virtual User UpdatedByUser { get; set; } // Navigation property to User (UpdatedBy)

        //public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        //public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
    }
}
