using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModels
{
    public class UpdateCoachScheduleViewModel
    {
        public int EnrollmentId { get; set; }
        //public string ScheduledAt { get; set; }
        //public decimal ScheduledHours { get; set; }
        public string Location { get; set; }

        // 👇 add these
        public int ChildId { get; set; }
        public int CourseId { get; set; }
        public int EnrollmentId_Ref { get; set; }
    }
}
