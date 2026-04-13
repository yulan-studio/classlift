using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.FormModels
{

    public class UpdateSchedulesFormModel
    {
        public int CourseID { get; set; }
        public List<ScheduleItemFormModel> Schedules { get; set; }
    }

    public class ScheduleItemFormModel
    {
        public int EnrollmentID { get; set; }
        public string Status { get; set; }
        public string ParentNote { get; set; }
    }
}