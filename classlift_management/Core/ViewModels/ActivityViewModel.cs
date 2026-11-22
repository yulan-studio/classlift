using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModels
{
    public class ActivityViewModel
    {
        public int ActivityID { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }

        public required string Address { get; set; }

        public bool IsActive { get; set; }
        public string ScheduleType { get; set; } = "OneTime";
        public DateTime? NextSessionAt { get; set; }
        public int SessionCount { get; set; }
        public DateTime ScheduledAt => NextSessionAt ?? DateTime.MinValue;
        public string Status => IsActive ? "Active" : "Inactive";
        public int MaxCapacity { get; set; }
        public decimal? Cost { get; set; }
        public int RegisteredChildrenCount { get; set; }
        public List<ActivitySessionListItemViewModel> Sessions { get; set; } = new();
    }

    public class ActivitySessionListItemViewModel
    {
        public int ActivitySessionID { get; set; }
        public DateTime ScheduledAt { get; set; }
        public int RegisteredCount { get; set; }
    }
}
