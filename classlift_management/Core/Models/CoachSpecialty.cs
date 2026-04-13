
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class CoachSpecialty
    {


        //[Key]
        //public int CoachSpecialtyID { get; set; } // Primary Key

        public int CoachID { get; set; } // Foreign Key to Coaches
        public int SpecialtyID { get; set; } // Foreign Key to Specialties

        //public int CreatedBy { get; set; } // Created By User
        //public int UpdatedBy { get; set; } // Updated By User

        //public DateTime CreatedDate { get; set; }  
        //public DateTime UpdatedDate { get; set; }

       

        // Foreign Key relationships
        [ForeignKey("CoachID")]
        public virtual Coach Coach { get; set; } // Navigation Property to Coach

        [ForeignKey("SpecialtyID")]
        public virtual Specialty Specialty { get; set; } // Navigation Property to Specialty

    }
}
