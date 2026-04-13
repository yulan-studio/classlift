using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModels
{
    public class CourseViewModel
    {
        public int CourseID { get; set; }

        public string SpecialtyName { get; set; }

        public string CoachName { get; set; }

        public string Title { get; set; }

        public string? Description { get; set; }

        public string CourseType { get; set; }

        public int? MaxCapacity { get; set; }

        public int? SessionCount { get; set; }

        public decimal? HourlyCost { get; set; }

        public decimal? HourlyCost2 { get; set; }

        public int RegisteredChildrenCount { get; set; }

        public bool IsActive { get; set; }
    }
}
