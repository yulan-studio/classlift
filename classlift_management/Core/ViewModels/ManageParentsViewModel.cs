using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models;

namespace Core.ViewModels
{
    public class ManageParentsViewModel
    {
        public Child Child { get; set; }
        public IEnumerable<ParentChild> Parents { get; set; }
    }
}
