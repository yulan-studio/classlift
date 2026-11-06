using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Report
{
    public class ChildCourseDto
    {
        public string CourseName { get; set; } = string.Empty;
        public int SessionsCompleted { get; set; }

        public DateTime? LastSessionDate { get; set; }
    }

    public class ChildReportDto
    {
        public string ChildName { get; set; } = string.Empty;
        public int TotalCourses { get; set; }
        public List<ChildCourseDto> Courses { get; set; } = new();
    }
}
