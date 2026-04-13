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
    public class ChildSchedulesViewModel
    {
        public Child Child { get; set; }

        public int ChildID { get; set; }
        public List<CourseSchedulesViewModel> CoursesSchedules { get; set; } = new();

        //public List<ActivitySchedulesViewModel> ActivitiesSchedules { get; set; } = new();

        public IEnumerable<ActivityEnrollment> ActivitySchedules { get; set; } 
    }

    //public class CourseSchedulesViewModel
    //{
    //    public Course Course { get; set; }

    //    public int CourseID { get; set; }
    //    public List<CourseEnrollment> Schedules { get; set; } = new();

    //    public Fee Fee { get; set; }

    //}


}