using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModels
{
    public class SessionAttendanceViewModel
    {
        public Core.Models.Course Course { get; set; }
        // List of all children in this course
        public List<ChildInfo> ChildrenList { get; set; } = new List<ChildInfo>();

        // List of sessions with enrolled children
        public List<SessionInfo> Sessions { get; set; } = new List<SessionInfo>();
    }

    public class ChildInfo
    {
        public int ChildID { get; set; }
        public string Name { get; set; } = "";
    }

    public class SessionInfo
    {
        public DateTime ScheduledAt { get; set; }

        // Dictionary: Key = ChildID, Value = Status (Scheduled/Completed/etc.)
        public Dictionary<int, string> Children { get; set; } = new Dictionary<int, string>();
    }
}
