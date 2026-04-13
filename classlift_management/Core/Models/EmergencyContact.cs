using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class EmergencyContact
    {
        public int EmergencyContactID { get; set; }
        public int? ChildID { get; set; }

        public int? CoachID { get; set; }

        public string ContactName { get; set; }
        public string? Relationship { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }

        public Child? Child { get; set; }

        public Coach? Coach { get; set; }

    }
}
