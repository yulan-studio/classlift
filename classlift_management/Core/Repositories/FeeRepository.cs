using Core.Interfaces;
using Core.Models;
using Core.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;




namespace Core.Repositories
{

    public class FeeRepository : IFeeRepository
    {
        


        private readonly AppDbContext _context;

        // Constructor to inject DbContext
        public FeeRepository(AppDbContext context)
        {
            _context = context;
        }

        // Add a new Fee
        public async Task<bool> AddAsync(Fee entity)
        {
            try
            {
                await _context.Fees.AddAsync(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // Remove a Specialty
        public async Task<bool> RemoveAsync(Fee entity)
        {
            try
            {
                _context.Fees.Remove(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }




        // Remove a Specialty
        public async Task<bool> DeleteCourseFeeAsync(int enrollmentId)
        {
            // Find all fees linked to this course enrollment
            var fees = await _context.Fees
                .Where(f => f.CourseEnrollmentID == enrollmentId)
                .ToListAsync();

            if (fees == null || !fees.Any())
                return false; // nothing to delete

            _context.Fees.RemoveRange(fees);

            // Save changes
            var rows = await _context.SaveChangesAsync();

            return rows > 0;
        }


        public async Task<bool> DeleteActivityFeeAsync(int enrollmentId)
        {
            // Find all fees linked to this course enrollment
            var fees = await _context.Fees
                .Where(f => f.ActivityEnrollmentID == enrollmentId)
                .ToListAsync();

            if (fees == null || !fees.Any())
                return false; // nothing to delete

            _context.Fees.RemoveRange(fees);

            // Save changes
            var rows = await _context.SaveChangesAsync();

            return rows > 0;
        }

        public async Task<Fee?> GetByCourseEnrollmentIdAsync(int courseEnrollmentId)
        {
            return await _context.Fees
                .Include(f => f.CourseEnrollment)
                .ThenInclude(e => e.Child)
                .FirstOrDefaultAsync(f => f.CourseEnrollmentID == courseEnrollmentId);
        }


        public async Task<Fee?> GetByChildIdCourseIdAsync(int childId, int courseId)
        {

            return await _context.Fees
                .Include(f => f.CourseEnrollment)
                    //.ThenInclude(e => e.Course)
                .Where(f => f.CourseEnrollment.ChildID == childId &&
                            f.CourseEnrollment.CourseID == courseId &&
                            f.CourseEnrollment.EnrollmentID_Ref == null &&
                            (f.CourseEnrollment.Status == "Registered"|| f.CourseEnrollment.Status == "Confirmed")
                            )
                .FirstOrDefaultAsync();

        }


        public async Task<Fee?> GetByActivityEnrollmentIdAsync(int activityEnrollmentId)
        {
            return await _context.Fees
                .Include(f => f.ActivityEnrollment)
                .ThenInclude(e => e.Child)
                .FirstOrDefaultAsync(f => f.ActivityEnrollmentID == activityEnrollmentId);
        }

        public async Task<Fee?> GetByChildIdActivityIdAsync(int childId, int activityId)
        {

            return await _context.Fees
                .Include(f => f.ActivityEnrollment)
                //.ThenInclude(e => e.Course)
                .Where(f => f.ActivityEnrollment.ChildID == childId &&
                            f.ActivityEnrollment.ActivityID == activityId &&
                            f.ActivityEnrollment.Status == "Registered"
                            )
                .FirstOrDefaultAsync();

        }









        // Update a Specialty
        public async Task<bool> UpdateAsync(Fee entity)
        {
            try
            {
                _context.Fees.Update(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // Get a Specialty by ID
        public async Task<Fee> GetAsync(int id)
        {
            return await _context.Fees
                //.Include(s => s.CreatedByUser)
                //.Include(s => s.UpdatedByUser)
                .FirstOrDefaultAsync(s => s.FeeID == id);
        }

    
        public async Task<IEnumerable<Fee>> GetAllAsync()
        {
            return await _context.Fees
                .ToListAsync();
        }


        public async Task<bool> UpdateActivityIsPaidAsync(int activityEnrollmentID, int userId)
        {
            try
            {
                var fee = await _context.Fees
                    .FirstOrDefaultAsync(e => e.ActivityEnrollmentID == activityEnrollmentID);

                if (fee == null)
                    return false;

                fee.IsPaid = true;

                // Optional tracking fields (only include if your table has these)
                fee.UpdatedBy = userId;
                fee.UpdatedAt = DateTimeHelper.GetTorontoTime();

                _context.Fees.Update(fee);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                // You can log the exception here if needed
                return false;
            }
        }


        public async Task<bool> UpdateCourseIsPaidAsync(int courseEnrollmentID, int userId)
        {
            try
            {
                var fee = await _context.Fees
                    .FirstOrDefaultAsync(e => e.CourseEnrollmentID == courseEnrollmentID);

                if (fee == null)
                    return false;

                fee.IsPaid = true;

                // Optional tracking fields (only include if your table has these)
                fee.UpdatedBy = userId;
                fee.UpdatedAt = DateTimeHelper.GetTorontoTime();

                _context.Fees.Update(fee);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                // You can log the exception here if needed
                return false;
            }
        }







    }








}
