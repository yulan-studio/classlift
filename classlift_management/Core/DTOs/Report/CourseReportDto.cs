using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Report
{

    public class CourseChildDto
    {
        public string ChildName { get; set; }
        public int SessionsCompleted { get; set; }
    }

    public class CourseReportDto
    {
        public string CourseName { get; set; }
        public int SessionsFinished { get; set; }
        //public List<string> Children { get; set; }

        public List<CourseChildDto> Children { get; set; }
    }
}
