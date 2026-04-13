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
   

    public class ManageCoachViewModel
    {
        public List<CoachWithDeleteViewModel> Coaches { get; set; } = new List<CoachWithDeleteViewModel>();
    }


    public class CoachWithDeleteViewModel
    {
        public Coach Coach { get; set; }
        public bool CanDelete{ get; set; }

    }


}