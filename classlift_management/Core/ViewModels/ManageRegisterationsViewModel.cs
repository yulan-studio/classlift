using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models;
using Core.ViewModels;

namespace Core.ViewModels
{
    public class ManageRegisterationsViewModel
    {
        public Child Child { get; set; }
        public IEnumerable<CourseEnrollmentViewModel> CourseEnrollments { get; set; }

        public IEnumerable<ActivityEnrollmentViewModel> ActivityEnrollments { get; set; }
    }
}