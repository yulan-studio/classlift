using Core.Contexts;
using Core.Interfaces;
using Core.Models;
using Core.ViewModels;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace Core.Repositories
{
    public class ChildBalanceRepository : IChildBalanceRepository
    {

        private readonly AppDbContext _context;

        // Constructor to inject DbContext
        public ChildBalanceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddBalanceAsync(Core.Models.ChildBalance balance)
        {
            _context.ChildBalances.Add(balance);
            return await _context.SaveChangesAsync() > 0;
        }


        public async Task<bool> AddPaymentToBalanceAsync(int childId, int paymentId, decimal amount, int createdBy)
        {
            decimal latestBalance = await GetFinalBalanceAsync(childId);

            var newEntry = new Core.Models.ChildBalance
            {
                ChildID = childId,
                PaymentID = paymentId,
                BalanceChange = amount,
                Balance = latestBalance + amount,
                TransactionType = "Payment",
                CreatedDate = DateTimeHelper.GetTorontoTime(),
                CreatedBy = createdBy,
                UpdatedBy = createdBy,
                UpdatedDate = DateTimeHelper.GetTorontoTime()
            };

            _context.ChildBalances.Add(newEntry);
            return await _context.SaveChangesAsync() > 0;
        }

       
        public async Task<bool> RemovePaymentToBalanceAsync(int childId, int paymentId, int createdBy)
        {
           
            _context.ChildBalances.RemoveRange(_context.ChildBalances.Where(cb => cb.ChildID == childId && cb.PaymentID == paymentId));
            return await _context.SaveChangesAsync() > 0;
        }


        //Deduct cost for a course session (Token pay)
        public async Task<bool> DeductCourseSessionCostAsync(int enrollmentId,  int createdBy)
        {
            var enrollment = await _context.CourseEnrollments
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.EnrollmentID == enrollmentId);

            decimal latestBalance = 0;

            if (enrollment != null && enrollment.ChildID != null)
            {
                latestBalance = await GetFinalBalanceAsync((int)enrollment.ChildID);
            }


            if (enrollment == null || enrollment.Status != "Completed")
                return false;

            // Calculate cost for this session only for private courses
            decimal costForThisSession = 0;
            if (enrollment.Course.HourlyCost != null && enrollment.ActualHours!= null)
                costForThisSession = (decimal)enrollment.Course.HourlyCost * (decimal)enrollment.ActualHours;


            

            var newEntry = new Core.Models.ChildBalance
            {
                ChildID = enrollment.ChildID,
                CourseID = enrollment.CourseID,
                EnrollmentID = enrollmentId,
                BalanceChange = costForThisSession*(-1),
                Balance = latestBalance - costForThisSession,
                TransactionType = "Course Session",
                CreatedDate = DateTimeHelper.GetTorontoTime(),
                CreatedBy = createdBy,
                UpdatedBy = createdBy,
                UpdatedDate = DateTimeHelper.GetTorontoTime()
            };

            _context.ChildBalances.Add(newEntry);
            return await _context.SaveChangesAsync() > 0;
        }

        //Deduct cost for a course (Direct pay)
        public async Task<bool> DeductCourseCostAsync(int childId, int courseId, decimal cost, int createdBy)
        {
            decimal latestBalance = await GetFinalBalanceAsync(childId);

            var newEntry = new Core.Models.ChildBalance
            {
                ChildID = childId,
                CourseID = courseId,
                BalanceChange = -cost,
                Balance = latestBalance - cost,
                TransactionType = "Course",
                CreatedDate = DateTimeHelper.GetTorontoTime(),
                //CreatedBy = createdBy,
                //UpdatedBy = createdBy,
                UpdatedDate = DateTimeHelper.GetTorontoTime()
            };

            _context.ChildBalances.Add(newEntry);
            return await _context.SaveChangesAsync() > 0;
        }



        public async Task<bool> DeductActivityCostAsync(int childId, int activityId, decimal cost, int createdBy)
        {
            decimal latestBalance = await GetFinalBalanceAsync(childId);

            var newEntry = new Core.Models.ChildBalance
            {
                ChildID = childId,
                ActivityID = activityId,
                BalanceChange = -cost,
                Balance = latestBalance - cost,
                TransactionType = "Activity",
                CreatedDate = DateTimeHelper.GetTorontoTime(),
                //CreatedBy = createdBy,
                //UpdatedBy = createdBy,
                UpdatedDate = DateTimeHelper.GetTorontoTime()
            };

            _context.ChildBalances.Add(newEntry);
            return await _context.SaveChangesAsync()>0;
        }


        public async Task<bool> DeductGroupCourseCostAsync(int childId, int courseId, decimal cost, int createdBy)
        {
            decimal latestBalance = await GetFinalBalanceAsync(childId);

            var newEntry = new Core.Models.ChildBalance
            {
                ChildID = childId,
                CourseID = courseId,
                BalanceChange = -cost,
                Balance = latestBalance - cost,
                CreatedDate = DateTimeHelper.GetTorontoTime(),
                TransactionType = "Course",
                //CreatedBy = createdBy,
                //UpdatedBy = createdBy,
                UpdatedDate = DateTimeHelper.GetTorontoTime()
            };

            _context.ChildBalances.Add(newEntry);
            return await _context.SaveChangesAsync() > 0;
        }





        public async Task<List<Core.ViewModels.ChildBalance>> GetBalanceHistoryAsync(int childId)
        {
            var history = await _context.ChildBalances
                .Where(cb => cb.ChildID == childId)
                .OrderBy(cb => cb.CreatedDate)
                .Select(cb => new Core.ViewModels.ChildBalance
                {
                    CreatedDate = cb.CreatedDate,
                    Type =  cb.TransactionType != null ? cb.TransactionType :"Other",
                    CourseName = cb.CourseID != null ? cb.Course.Title : null,
                    ActivityName = cb.ActivityID != null ? cb.Activity.Title : null,
                    BalanceChange = cb.BalanceChange ?? 0,
                    Balance = cb.Balance ?? 0,
                    Remarks = cb.Remarks,
                    Calculation = cb.Calculation,

                    ScheduledAt = cb.EnrollmentID != null ? cb.CourseEnrollment.ScheduledAt : null,
                    ActualHours = cb.EnrollmentID != null ? cb.CourseEnrollment.ActualHours : null
                })
                .ToListAsync();

            return history;
        }


        //public async Task<decimal> GetFinalBalanceAsync(int childId)
        //{
        //    var latest = await _context.ChildBalances
        //        .Where(cb => cb.ChildID == childId)
        //        .OrderByDescending(cb => cb.CreatedDate)
        //        .FirstOrDefaultAsync();

        //    return latest?.Balance ?? 0;
        //}


        public async Task<decimal> GetFinalBalanceAsync(int childId)
        {
            return await _context.ChildBalances
                .Where(cb => cb.ChildID == childId)
                .OrderByDescending(cb => cb.CreatedDate)
                .Select(cb => cb.Balance ?? 0)
                .FirstOrDefaultAsync();
        }
    }


}
