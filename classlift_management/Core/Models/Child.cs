
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Child
    {
        //[Key]

        public string? MemberID { get; set; } = string.Empty; // Unique member ID
        public int ChildID { get; set; } // Primary key

        public int UserID { get; set; }

        public virtual User User { get; set; }

        // Child's name
        [MaxLength(255)]
        public required string Name { get; set; } = string.Empty;

        // Child's birth date
        public DateTime? BirthDate { get; set; }

        // Child's gender
        public string? Gender { get; set; }

        // Child's city
        public int? CityID { get; set; }
        [ForeignKey("CityID")]
        public virtual required City City { get; set; } // Navigation property to Speical table (SpecialID)

        public string? PrimaryDiagnosis { get; set; }
        public string? Address { get; set; }
        public int? OAPAmount { get; set; }
        public bool PhotoConsent { get; set; }

        //public List<EmergencyContact> EmergencyContacts { get; set; }
        public ICollection<EmergencyContact>? EmergencyContacts { get; set; } = new List<EmergencyContact>();



        // ✅ New: List of related parents
        public virtual ICollection<ParentChild>? ParentChild { get; set; } = new List<ParentChild>();

        //if child has OAP funding
        public bool HasOAP { get; set; }

        [DataType(DataType.MultilineText)]
        public string? Notes { get; set; }

    }
}
