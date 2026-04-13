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
    public class EnrollmentsHistoryViewModel
    {
        public Child Child { get; set; }
        
        public List<CourseEnrollment>? CompletedCourses { get; set; }

        public List<ActivityEnrollment>? CompletedActivities { get; set; }
    }
}