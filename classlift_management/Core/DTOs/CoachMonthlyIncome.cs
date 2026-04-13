using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class CoachMonthlyIncome
    {
        public int Year { get; set; }
        public int Month { get; set; }   // 1–12
        public decimal TotalHours { get; set; }
        //public decimal TotalIncome { get; set; }

        public List<CoachIncomeDetail> Details { get; set; } = new();
    }

    public class CoachIncomeDetail
    {
        public DateTime ScheduledAt { get; set; }
        public decimal Hours { get; set; }
        //public decimal Amount { get; set; }

        public string ChildName { get; set; }
        public string CourseName { get; set; }
    }
}
