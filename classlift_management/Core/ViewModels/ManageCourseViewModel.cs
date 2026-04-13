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
   

    public class ManageCourseViewModel
    {

        //public Course Course { get; set; }
        //public List<RegisteredChild> RegisteredChildren { get; set; }
        [Required]
        public Coach Coach { get; set; }

        public List<SpecialtyCoursesViewModel> Specialties { get; set; } = new List<SpecialtyCoursesViewModel>();
    }


    public class SpecialtyCoursesViewModel
    {
        public int SpecialtyID { get; set; }
        public string SpecialtyTitle { get; set; }

        //[Required]
        //public Specialty Specialty { get; set; }

        public List<CourseChildrenViewModel> Courses { get; set; } = new List<CourseChildrenViewModel>();
    }


    public class CourseChildrenViewModel
    {

        //[Required]
        //public Course Course { get; set; }

        public int CourseID { get; set; }
        public string CourseTitle { get; set; }

        public string CourseType { get; set; }

        public string? CourseDescription { get; set; }

        public int? SessionCount { get; set; }

        public List<ChildViewModel> RegisteredChildren { get; set; } = new List<ChildViewModel>();
    }


    public class ChildViewModel
    {
        public int EnrollmentID { get; set; }
        public int ChildID { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string City { get; set; }
        public int Scheduled { get; set; }

        public int Deleted { get; set; }
        public int Completed { get; set; }

        public int RequestToReschedule { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime RegisteredDate { get; set; }
    }
}