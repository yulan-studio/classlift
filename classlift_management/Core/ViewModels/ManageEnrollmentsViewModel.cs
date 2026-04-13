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
    public class ManageEnrollmentsViewModel
    {
        public Child Child { get; set; }
        public Course Course { get; set; }
        public List<CourseEnrollment> WaitToCompleteEnrollments { get; set; }

        public List<CourseEnrollment> CompletedEnrollments { get; set; }

        public List<CourseEnrollment> DeletedEnrollments { get; set; }
    }
}