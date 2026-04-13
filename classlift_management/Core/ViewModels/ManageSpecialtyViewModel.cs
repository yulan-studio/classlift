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
   

    public class ManageSpecialtyViewModel
    {
        public List<SpecialtyWithDeleteViewModel> SpecialtiesWithDelete { get; set; } = new List<SpecialtyWithDeleteViewModel>();
    }


    public class SpecialtyWithDeleteViewModel
    {
        public Specialty Specialty { get; set; }
        public bool CanDelete{ get; set; }

        
    }


}