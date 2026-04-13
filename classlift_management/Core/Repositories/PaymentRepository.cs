using Core.Contexts;
using Core.Interfaces;
using Core.Models;
using Core.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace Core.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _context;

        public PaymentRepository(AppDbContext context)
        {
            _context = context;
        }

        // 🔹 Get all payments
        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            return await _context.Payments
                .Include(p => p.Parent)
                .Include(p => p.PaymentPackage)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetByChildAsync(int childId)
        {
            return await _context.Payments
                .Include(p => p.Parent)
                .Include(p => p.PaymentPackage)
                .Include(p => p.Fee)
                .ThenInclude(Fee => Fee.ActivityEnrollment) // ✅ Include ActivityEnrollment
                .ThenInclude(ae => ae.Activity) // ✅ Include Activity entity
                .Include(p => p.Fee)
                .ThenInclude(Fee => Fee.CourseEnrollment) // ✅ Include CourseEnrollment
                .ThenInclude(ce => ce.Course) // ✅ Include Course entity
                .Include(p => p.Parent.ParentChild) // ✅ Include ParentChild relationship
                .ThenInclude(pc => pc.Child) // ✅ Include Child entity
                .Where(p => _context.ParentChild.Any(pc => pc.ChildID == childId && pc.ParentID == p.ParentID)) // ✅ Filters by childId using ParentChild
                .ToListAsync();
        }

        // 🔹 Get payment by ID
        public async Task<Payment> GetByIdAsync(int id)
        {
            return await _context.Payments
                .Include(p => p.Parent)
                .Include(p => p.PaymentPackage)
                .Include(p => p.Fee)
                .FirstOrDefaultAsync(p => p.PaymentID == id);
        }

        public async Task<IEnumerable<Payment>> GetByPackageAsync(int packageId)
        {
            return await _context.Payments
                .Include(p => p.PaymentPackage)
                .Where(p => p.PaymentPackage.PackageID == packageId)
                .ToListAsync();
        }


        public async Task<Child> GetChildByIdAsync(int childId)
        {
            return await _context.Children
                .Where(c => c.ChildID == childId)
                .FirstOrDefaultAsync();
        }


        public async Task<IEnumerable<Parent>> GetParentsByChildAsync(int childId)
        {
            return await _context.ParentChild
                .Where(pc => pc.ChildID == childId)
                .Select(pc => pc.Parent)
                .ToListAsync();
        }

        public async Task<IEnumerable<PaymentPackage>> GetAllActivePackagesAsync()
        {
            return await _context.PaymentPackages
                .Where(p => p.IsActive) // ✅ Only fetch active packages
                .ToListAsync();
        }

        // 🔹 Add a new payment
        public async Task<int> AddPaymentAsync(Payment payment)
        {
            
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
            return payment.PaymentID;
           
         
        }

        // 🔹 Update an existing payment
        public async Task<bool> UpdateAsync(Payment payment)
        {
            try
            {
                var existingPayment = await _context.Payments.FindAsync(payment.PaymentID);
                if (existingPayment == null) return false;

                _context.Entry(existingPayment).CurrentValues.SetValues(payment);
                var changes = await _context.SaveChangesAsync();
                return changes >= 0; // Returns true even if 0 rows were affected
            }
            catch (Exception)
            {
                return false;
            }
        }

        // 🔹 Delete a payment
        public async Task<bool> RemoveAsync(int id)
        {
            var payment = await GetByIdAsync(id);
            if (payment == null) return false;

            try
            {
                _context.Payments.Remove(payment);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public async Task<List<UnpaidItemViewModel>> GetUnpaidDirectEnrollmentsByChildAsync(int childId)
        {
            var unpaidCourses = await (from f in _context.Fees
                                       join ce in _context.CourseEnrollments on f.CourseEnrollmentID equals ce.EnrollmentID
                                       join c in _context.Courses on ce.CourseID equals c.CourseID
                                       where ce.ChildID == childId
                                             && f.PaymentModel == "Direct"
                                             && (f.IsPaid == false || f.IsPaid == null)
                                       select new UnpaidItemViewModel
                                       {
                                           Type = "Course",
                                           Title = c.Title,
                                           TotalCost = f.TotalCost,
                                           FeeID = f.FeeID
                                       }).ToListAsync();

            var unpaidActivities = await (from f in _context.Fees
                                          join ae in _context.ActivityEnrollments on f.ActivityEnrollmentID equals ae.EnrollmentID
                                          join a in _context.Activities on ae.ActivityID equals a.ActivityID
                                          where ae.ChildID == childId
                                                && f.PaymentModel == "Direct"
                                                && (f.IsPaid == false || f.IsPaid == null)
                                          select new UnpaidItemViewModel
                                          {
                                              Type = "Activity",
                                              Title = a.Title,
                                              TotalCost = f.TotalCost,
                                              FeeID = f.FeeID
                                          }).ToListAsync();

            // Combine both lists
            return unpaidCourses.Concat(unpaidActivities).ToList();
        }




        public async Task<List<UnpaidItemViewModel>> GetUnpaidOAPEnrollmentsByChildAsync(int childId)
        {
            var unpaidCourses = await (from f in _context.Fees
                                       join ce in _context.CourseEnrollments on f.CourseEnrollmentID equals ce.EnrollmentID
                                       join c in _context.Courses on ce.CourseID equals c.CourseID
                                       where ce.ChildID == childId
                                             && f.PaymentModel == "OAP"
                                             && (f.IsPaid == false || f.IsPaid == null)
                                       select new UnpaidItemViewModel
                                       {
                                           Type = "Course",
                                           Title = c.Title,
                                           TotalCost = f.TotalCost,
                                           FeeID = f.FeeID
                                       }).ToListAsync();

            var unpaidActivities = await (from f in _context.Fees
                                          join ae in _context.ActivityEnrollments on f.ActivityEnrollmentID equals ae.EnrollmentID
                                          join a in _context.Activities on ae.ActivityID equals a.ActivityID
                                          where ae.ChildID == childId
                                                && f.PaymentModel == "OAP"
                                                && (f.IsPaid == false || f.IsPaid == null)
                                          select new UnpaidItemViewModel
                                          {
                                              Type = "Activity",
                                              Title = a.Title,
                                              TotalCost = f.TotalCost,
                                              FeeID = f.FeeID
                                          }).ToListAsync();

            // Combine both lists
            return unpaidCourses.Concat(unpaidActivities).ToList();
        }


        //public async Task<bool> AddDirectPaymentAsync(int feeId, int createdBy)
        //{
        //    var fee = await _context.Fees.FirstOrDefaultAsync(f => f.FeeID == feeId);
        //    if (fee == null) return false;

        //    // 1. Add payment record
        //    var payment = new Payment
        //    {
        //        FeeID = fee.FeeID,
        //        Amount = fee.TotalCost,
        //        CreatedBy = createdBy,
        //        CreatedDate = DateTime.UtcNow
        //    };

        //    _context.Payments.Add(payment);

        //    // 2. Mark fee as paid
        //    fee.IsPaid = true;

        //    await _context.SaveChangesAsync();
        //    return true;
        //}



    }
}
