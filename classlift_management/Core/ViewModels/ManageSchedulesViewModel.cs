using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Core.ViewModels
{
    public class ManageSchedulesViewModel
    {
        public int EnrollmentID { get; set; }
        public Child Child { get; set; }

        public IEnumerable<ParentChild> Parents { get; set; }

        public Course Course { get; set; }

        public int ScheduledCount { get; set; }

        public int CompletedCount { get; set; }


        public List<CourseEnrollment> Schedules { get; set; }
    }
}