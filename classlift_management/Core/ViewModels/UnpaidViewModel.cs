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

    public class UnpaidItemViewModel
    {
        public string Type { get; set; } // "Course" or "Activity"
        public string Title { get; set; }
        public decimal? TotalCost { get; set; }

        public int FeeID { get; set; }
    }
}