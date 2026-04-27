using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Report
{
    public class StudentCourseCountDto
    {
        public int ChildId { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
    }
}
