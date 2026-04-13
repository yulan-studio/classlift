using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.FormModels
{

    public class UpdateSchedulesToConfirmFormModel
    {
        public int CourseID { get; set; }

        public Fee Fee { get; set; }
        public List<ScheduleToConfirmItemFormModel> Schedules { get; set; }
    }

    public class ScheduleToConfirmItemFormModel
    {
        public int EnrollmentID { get; set; }
        public string ParentNote { get; set; }
    }
}