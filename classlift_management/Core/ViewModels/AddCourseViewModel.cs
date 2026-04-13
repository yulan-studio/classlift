using System.ComponentModel.DataAnnotations;

namespace Core.ViewModels
{
    public class AddCourseViewModel
    {
        [Required]
        public int SpecialtyID { get; set; }

        [Required]
        public int CoachID { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        public string? Description { get; set; }

        public string CourseType { get; set; }

        public int? MaxCapacity { get; set; }

        public int? SessionCount { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal HourlyCost { get; set; }

       
        [Range(0, double.MaxValue)]
        public decimal? HourlyCost2 { get; set; }

        public bool IsActive { get; set; }
    }

}
