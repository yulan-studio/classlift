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
   

    public class ManagePackageViewModel
    {
        public List<PackageWithDeleteViewModel> Packages { get; set; } = new List<PackageWithDeleteViewModel>();
    }


    public class PackageWithDeleteViewModel
    {
        public PaymentPackage Package { get; set; }
        public bool CanDelete{ get; set; }

        
    }


}