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

        public int MaxCapacity { get; set; }
        public DateTime ScheduledAt { get; set; }
        //public decimal Cost { get; set; }

        //public bool IsActive { get; set; }

        public required string Status { get; set; }
        public int RegisteredChildrenCount { get; set; }
    }
}
