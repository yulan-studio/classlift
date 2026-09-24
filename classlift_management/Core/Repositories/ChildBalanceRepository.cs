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

        public async Task<bool> RefundCanceledSessionCostAsync(int sessionEnrollmentId, int createdBy)
        {
            var session = await _context.CourseEnrollments
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.EnrollmentID == sessionEnrollmentId && e.Status == "Canceled");
            if (session?.Course?.SessionCost is not decimal sessionCost || sessionCost <= 0)
                return false;

            var registrations = await _context.CourseEnrollments
                .Where(e => e.EnrollmentID_Ref == sessionEnrollmentId && e.ChildID != null && e.Status == "Canceled")
                .ToListAsync();
            if (registrations.Count == 0)
                return false;

            var registrationIds = registrations.Select(e => e.EnrollmentID).ToList();
            var alreadyRefunded = await _context.ChildBalances
                .Where(b => b.EnrollmentID != null && registrationIds.Contains(b.EnrollmentID.Value)
                    && b.TransactionType == "Refund")
                .Select(b => b.EnrollmentID!.Value)
                .ToListAsync();

            var balanceByChild = new Dictionary<int, decimal>();
            var eligibleRegistrations = registrations
                .Where(r => !alreadyRefunded.Contains(r.EnrollmentID))
                .ToList();
            if (eligibleRegistrations.Count == 0)
                return false;

            foreach (var registration in eligibleRegistrations)
            {
                var childId = registration.ChildID!.Value;
                if (!balanceByChild.TryGetValue(childId, out var currentBalance))
                    currentBalance = await GetFinalBalanceAsync(childId);

                var newBalance = currentBalance + sessionCost;
                _context.ChildBalances.Add(new Core.Models.ChildBalance
                {
                    ChildID = childId,
                    CourseID = session.CourseID,
                    EnrollmentID = registration.EnrollmentID,
                    TransactionType = "Refund",
                    Remarks = $"Refund for canceled session: {session.Course.Title} on {session.ScheduledAt?.ToString("yyyy-MM-dd") ?? "unknown date"}",
                    BalanceChange = sessionCost,
                    Balance = newBalance,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = createdBy,
                    UpdatedBy = createdBy,
                    UpdatedDate = DateTime.UtcNow
                });
                balanceByChild[childId] = newBalance;
            }

            // A refund means this canceled session will not be replaced, so it
            // must no longer be part of the course's final session count.
            if (session.Course.SessionCount is int sessionCount && sessionCount > 0)
                session.Course.SessionCount = sessionCount - 1;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<int>> GetRefundedSessionEnrollmentIdsAsync(int courseId)
        {
            var refundedChildEnrollmentIds = _context.ChildBalances
                .Where(b => b.CourseID == courseId
                    && b.EnrollmentID != null
                    && b.TransactionType == "Refund")
                .Select(b => b.EnrollmentID!.Value);

            return await _context.CourseEnrollments
                .Where(e => e.CourseID == courseId
                    && e.EnrollmentID_Ref != null
                    && refundedChildEnrollmentIds.Contains(e.EnrollmentID))
                .Select(e => e.EnrollmentID_Ref!.Value)
                .Distinct()
                .ToListAsync();
        }


        public async Task<bool> AddPaymentToBalanceAsync(int childId, int paymentId, decimal amount, string fileUrl, int createdBy)
        {
            decimal latestBalance = await GetFinalBalanceAsync(childId);

            var newEntry = new Core.Models.ChildBalance
            {
                ChildID = childId,
                PaymentID = paymentId,
                BalanceChange = amount,
                Balance = latestBalance + amount,
                TransactionType = "Payment",
                Calculation = fileUrl,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = createdBy,
                UpdatedBy = createdBy,
                UpdatedDate = DateTime.UtcNow
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
                CreatedDate = DateTime.UtcNow,
                CreatedBy = createdBy,
                UpdatedBy = createdBy,
                UpdatedDate = DateTime.UtcNow
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
                CreatedDate = DateTime.UtcNow,
                CreatedBy = createdBy,
                UpdatedBy = createdBy,
                UpdatedDate = DateTime.UtcNow
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
                CreatedDate = DateTime.UtcNow,
                //CreatedBy = createdBy,
                //UpdatedBy = createdBy,
                UpdatedDate = DateTime.UtcNow
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
                CreatedDate = DateTime.UtcNow,
                TransactionType = "Course",
                CreatedBy = createdBy,
                UpdatedBy = createdBy,
                UpdatedDate = DateTime.UtcNow
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
                    Type = cb.TransactionType != null ? cb.TransactionType : "Other",
                    CourseName = cb.CourseID != null ? cb.Course.Title : null,
                    ActivityName = cb.ActivityID != null ? cb.Activity.Title : null,
                    BalanceChange = cb.BalanceChange ?? 0,
                    Balance = cb.Balance ?? 0,
                    StaffName = cb.CreatedBy == null || cb.CreatedBy == 0
                        ? "System"
                        : (_context.Staff.Where(s => s.UserID == cb.CreatedBy).Select(s => s.Name).FirstOrDefault()
                            ?? _context.Coaches.Where(c => c.UserID == cb.CreatedBy).Select(c => c.Name).FirstOrDefault()
                            ?? _context.Users.Where(u => u.Id == cb.CreatedBy).Select(u => u.UserName).FirstOrDefault()
                            ?? "System"),
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
