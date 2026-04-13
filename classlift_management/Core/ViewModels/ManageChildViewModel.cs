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
   

    public class ManageChildViewModel
    {
        public List<ChildWithDeleteViewModel> Children { get; set; } = new List<ChildWithDeleteViewModel>();
    }


    public class ChildWithDeleteViewModel
    {
        public Child Child { get; set; }
        public bool CanDelete{ get; set; }

        
    }


}