using Core.DTOs;
using Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IChildCalendarRepository
    {
        Task<IEnumerable<CalendarSchedule>> GetChildCalendarEvents(int childId);
    }
}
