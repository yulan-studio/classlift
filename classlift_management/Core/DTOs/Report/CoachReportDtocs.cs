using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Report
{

    public class CoachCourseDto
    {
        public string CourseName { get; set; }
        public int SessionsFinished { get; set; }
        public List<string> Children { get; set; }
    }

    public class CoachReportDto
    {
        public string CoachName { get; set; }
        public int TotalCourses { get; set; }
        public List<CoachCourseDto> Courses { get; set; }
    }
}
