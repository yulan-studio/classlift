
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Course
    {
        [Key]
        public int CourseID { get; set; } // Primary key for the table

        [Required]
        [StringLength(255)]
        public string Title { get; set; } // Title of the course

        
        public string? Description { get; set; } // Description of the course

        [Required]
        public string CourseType { get; set; } // 'Group' or 'Private' 

        
        public int? MaxCapacity { get; set; } // MaxCapacity For Group class  

      
        public int? SessionCount { get; set; } // SessionCount For Group class  


        //[Required]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter a valid hourly cost.")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal? HourlyCost { get; set; }  // Hourly cost of the course


        //[Required]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter a valid hourly cost for child without OAP funding.")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal? HourlyCost2 { get; set; }  // Hourly cost of the course for child without OAP funding


        [Required]
        public bool IsActive { get; set; } = true; // Whether the course is active or not

        // Foreign keys for related tables
        
        public int? CoachID { get; set; } // Foreign key to the Coach table
        [Required]
        public int SpecialtyID { get; set; }

        public int CreatedBy { get; set; } // Foreign key to the User table for the creator
        public int? UpdatedBy { get; set; } // Foreign key to the User table for the last updater

        // Timestamps for record creation and update
        public DateTime CreatedDate { get; set; } 
        public DateTime? UpdatedDate { get; set; } 

        // Navigation properties
        [ForeignKey(nameof(CoachID))]
        public virtual Coach? Coach { get; set; } // Correctly mapped to Coach

        [ForeignKey(nameof(SpecialtyID))]
        public virtual required Specialty Specialty { get; set; }

        [ForeignKey(nameof(CreatedBy))]
        public virtual  User CreatedByUser { get; set; } // Navigation property for the user who created the course

        [ForeignKey(nameof(UpdatedBy))]
        public virtual User? UpdatedByUser { get; set; } // Navigation property for the user who last updated the course
    }
}
