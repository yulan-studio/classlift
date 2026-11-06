using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class CalendarSchedule
    {
        public string Title { get; set; } = string.Empty;
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Status { get; set; } = string.Empty;     // Scheduled / Completed
        public string Color { get; set; } = string.Empty;      // FullCalendar 用
        public string Type { get; set; } = string.Empty; // Course / Activity
    }
}
