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
   

    public class CourseSchedulesViewModel
    {
        public Course Course { get; set; }

        public int CourseID { get; set; }
        public List<CourseEnrollment> Schedules { get; set; } = new();

        public Fee Fee { get; set; }

    }


}