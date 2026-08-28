using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.FormModels
{

    public class UpdateAllSessionsFormModel
    {
        public int ChildID { get; set; }
        public int CourseID { get; set; }
        public List<SessionItemFormModel> AllSessions { get; set; } = new();
    }

    public class SessionItemFormModel
    {
        public int EnrollmentID { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? StaffNote { get; set; }
    }
}
