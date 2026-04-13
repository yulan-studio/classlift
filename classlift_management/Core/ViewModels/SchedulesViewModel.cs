using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Core.Interfaces;

namespace Core.ViewModels
{

    //This is for group courses, for staff to add sessions in group course
    public class ManageSessionsViewModel
    {
        public required Course Course { get; set; }
        public List<CourseEnrollment>? OpenSessions { get; set; }

        public List<CourseEnrollment>? ClosedSessions { get; set; }

        //public List<CourseEnrollment>? CanceledSessions { get; set; }

        public List<CourseEnrollment>? FinishedSessions { get; set; }

        public List<CourseEnrollment>? AllUpcomingSessions { get; set; }

        // ✅ New Property: List of Session IDs that are registered
        public List<int?> RegisteredUpcomingSessionIds { get; set; } = new List<int?>();

        
    }
}