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
    public class ManageSessionRegistrationsViewModel
    {
        public int ChildID { get; set; }
        public int CourseID { get; set; }

        public Child Child { get; set; }
        public Course Course { get; set; }

        public List<SessionViewModel> AllSessions { get; set; } = new();


        //public List<SessionViewModel> ScheduledSessions { get; set; } = new();




        public class SessionViewModel
        {
            public int EnrollmentID { get; set; }
            public DateTime ScheduledAt { get; set; }
            public decimal ScheduledHours { get; set; }
            public required string Status { get; set; }
            public string? ParentNote { get; set; }
            public string? StaffNote { get; set; }
        }


            
           
       
    }
}
