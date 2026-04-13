using Core.Interfaces;
using Core.Models;
using Core.DTOs;
using Core.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;




namespace Core.Repositories
{
    public class CoachIncomeRepository : ICoachIncomeRepository
    {

        private readonly AppDbContext _context;

        // Constructor to inject DbContext
        public CoachIncomeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> UpdateCoachIncomeAsync(int enrollmentId, int updatedBy)
        {
            var enrollment = await _context.CourseEnrollments
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.EnrollmentID == enrollmentId);

            if (enrollment == null || enrollment.Status != "Completed")
                return false;

            var coachId = enrollment.Course.CoachID;
            var courseId = enrollment.Course.CourseID;


            // Calculate income (replace with your logic — hardcoded here as example)
            //decimal incomeForThisSession = (decimal)enrollment.Course.HourlyCost * (decimal)enrollment.ActualHours;

            // Get latest income for this coach
            //decimal previousIncome = await _context.CoachIncomes
            //    .Where(i => i.CoachID == coachId)
            //    .OrderByDescending(i => i.IncomeID)
            //    .Select(i => i.Income ?? 0)
            //    .FirstOrDefaultAsync();

            //var newIncome = previousIncome + incomeForThisSession;

            var incomeEntry = new CoachIncome
            {
                CoachID = (int)coachId,
                CourseID = courseId,
                EnrollmentID = enrollmentId,
                //IncomeChange = incomeForThisSession,
                //Income = newIncome,
                CreatedDate = DateTimeHelper.GetTorontoTime(),
                CreatedBy = updatedBy
            };

            _context.CoachIncomes.Add(incomeEntry);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<IEnumerable<CoachIncome>> GetCoachIncomeAsync(int coachId)
        {

            var incomeRecords = await _context.CoachIncomes
                .Where(i => i.CoachID == coachId)
                .Include(i => i.Enrollment)
                    .ThenInclude(e => e.Child)
                .Include(i => i.Course)
                .OrderBy(i => i.Enrollment.ScheduledAt)
                .ToListAsync();
            return incomeRecords;
        }



        public async Task<IEnumerable<CoachMonthlyIncome>> GetCoachMonthlyIncomeAsync(int coachId)
        {
            var records = await _context.CoachIncomes
                .Where(i => i.CoachID == coachId)
                .Include(i => i.Enrollment)
                    .ThenInclude(e => e.Child)
                .Include(i => i.Course)
                .ToListAsync();

            // Fix for CS1061: Safely access Year and Month from nullable DateTime (DateTime?)
            var result = records
                .GroupBy(i => new
                {
                    Year = ((DateTime)(i.Enrollment.ScheduledAt).Value).Year,
                    Month = ((DateTime)(i.Enrollment.ScheduledAt).Value).Month

                })
                .OrderByDescending(g => g.Key.Year)
                .ThenByDescending(g => g.Key.Month)
                .Select(g => new CoachMonthlyIncome
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalHours = (decimal)g.Sum(x => x.Enrollment.ActualHours),
                    //TotalIncome = g.Sum(x => x.Amount),
                    Details = g.OrderBy(x => x.Enrollment.ScheduledAt).Select(x => new CoachIncomeDetail
                    {
                        ScheduledAt = (DateTime)x.Enrollment.ScheduledAt,
                        Hours = (decimal)x.Enrollment.ActualHours,
                        //Amount = x.Amount,
                        ChildName = x.Enrollment.Child != null ? x.Enrollment.Child.Name : "Group",
                        CourseName = x.Course.Title
                    }).ToList()
                })
                .ToList();

            return result;
        }
    }


}
