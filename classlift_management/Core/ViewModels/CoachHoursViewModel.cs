using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DTOs;
using Core.Models;

namespace Core.ViewModels
{
    
    

    public class CoachHoursViewModel
    {
        public required Coach Coach { get; set; }
        public List<CoachMonthlyIncome>? MonthlyIncomes { get; set; }
    }
}
