using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class CalendarSchedule
    {
        public string Title { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Status { get; set; }     // Scheduled / Completed
        public string Color { get; set; }      // FullCalendar 用
        public string Type { get; set; } // Course / Activity
    }
}
