using Core.Contexts;
using Core.DTOs;
using Core.Interfaces;
using Core.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories
{
    public class ChildCalendarRepository: IChildCalendarRepository
    {
        

        private readonly AppDbContext _context;

        public ChildCalendarRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CalendarSchedule>> GetChildCalendarEvents(int childId)
        {
            // Course events
            var courseEvents = await _context.CourseEnrollments
                .Where(e => e.ChildID == childId && e.ScheduledAt != null && e.Status !="Deleted")
                .Include(e => e.Course)
                .ThenInclude(c => c.Coach)
                .Select(e => new CalendarSchedule
                {
                    Title = e.Course.Title,
                    Start = e.ScheduledAt.Value,
                    End = e.ActualHours != null
                    ? e.ScheduledAt.Value.AddHours((double)e.ActualHours.Value)
                    : e.ScheduledAt.Value.AddHours((double)(e.ScheduledHours ?? 0)),
                    
                    Status = e.Status,
                    Type = "Course",
                    Color = GetColor(e.Status)
                })
                .ToListAsync();

            

            var activityEvents = await _context.ActivityEnrollments
            .Where(e => e.ChildID == childId && e.Activity.ScheduledAt != null)
            .Include(e => e.Activity)
            .Select(e => new CalendarSchedule
            {
                Title = e.Activity.Title,
                Start = e.Activity.ScheduledAt,
                End = e.Activity.ScheduledAt.AddHours((double)(e.Activity.ScheduledHours ?? 1m)),
                Status = e.Status,
                Type = "Activity",
                Color = GetColor(e.Status)
            })
            .ToListAsync();

            return courseEvents.Concat(activityEvents).ToList();

            //return courseEvents.ToList();
        }

        private static string GetColor(string status)
        {
            return status switch
            {
                "Scheduled" => "#007bff",
                "Completed" => "#28a745",
                "Canceled" => "#dc3545",
                "OnLeave" => "#ffc107",
                _ => "#6c757d"
            };
        }
    }
}
