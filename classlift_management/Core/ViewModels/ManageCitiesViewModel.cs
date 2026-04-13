using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModels
{
    public class ManageCitiesViewModel
    {
        public IEnumerable<City> Cities { get; set; }
        public HashSet<int> UsedCityIds { get; set; }
    }

}
