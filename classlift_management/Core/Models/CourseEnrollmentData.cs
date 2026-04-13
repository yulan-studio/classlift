using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class CourseEnrollmentData
    {
        public DateTime ScheduledAt { get; set; }
        public int? ChildID { get; set; }
        public string Status { get; set; }
    }
}
