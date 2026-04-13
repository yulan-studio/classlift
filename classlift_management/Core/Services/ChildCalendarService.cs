using Core.DTOs;
using Core.Interfaces;
using Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public class ChildCalendarService: IChildCalendarService
    {
        private readonly IChildCalendarRepository _repository;

        public ChildCalendarService(IChildCalendarRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<CalendarSchedule>> GetChildCalendar(int childId)
        {
            return await _repository.GetChildCalendarEvents(childId);

        }
    }
}
