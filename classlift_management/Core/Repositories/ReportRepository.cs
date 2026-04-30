using Core.Contexts;
using Core.Interfaces;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Core.Repositories
{
    public class ReportRepository : IReportRepository
    {

        private readonly AppDbContext _context;

        public ReportRepository(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<CourseEnrollment> GetCompletedEnrollments(DateTime? from, DateTime? to)
        {
            var query = _context.CourseEnrollments
                .Include(e => e.Child)
                .Include(e => e.Course)
                    .ThenInclude(c => c.Coach)
                .Where(e => e.Status == "Completed" && e.Child != null && e.ActualHours != null);

            if (from.HasValue)
            {
                query = query.Where(e => e.ScheduledAt >= from.Value);
            }

            if (to.HasValue)
            {
                query = query.Where(e => e.ScheduledAt <= to.Value);
            }

            return query;
        }

    }
}
