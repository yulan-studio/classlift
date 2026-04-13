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
    public class ChildRepository : IChildRepository
    {
        private readonly AppDbContext _context;

        public ChildRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Child>> GetAllAsync()
        {
            return await _context.Children
                .Include(c => c.City)
                .Include(c => c.User)
                .ToListAsync();
        }

        public async Task<Child?> GetAsync(int childId)
        {
            return await _context.Children
                .Include(c => c.City)
                .Include(c => c.User)
                .Include(c => c.EmergencyContacts)
                .Include(c => c.ParentChild)
                    .ThenInclude(pc => pc.Parent)
                .FirstOrDefaultAsync(c => c.ChildID == childId);
        }

        public async Task<Child?> GetByIdAsync(int userId)
        {
            return await _context.Children
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.User.Id == userId);
        }

        public async Task<bool> AddAsync(Child entity)
        {
            await _context.Children.AddAsync(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(Child entity)
        {
            _context.Entry(entity.User).State = EntityState.Modified;
            _context.Children.Update(entity);
            
            var changes = await _context.SaveChangesAsync();
            return changes >= 0; // Returns true even if 0 rows were affected
        }

        //public async Task<bool> DeleteAsync(int usrId)
        //{
        //    var child = await _context.Children.FindAsync(id);
        //    if (child == null) return false;

        //    _context.Children.Remove(child);
        //    return await _context.SaveChangesAsync() > 0;
        //}

        public async Task<bool> RemoveAsync(Child entity)
        {
            try
            {
                _context.Children.Remove(entity);
                await _context.SaveChangesAsync();  // Commit the changes asynchronously
                return true;
            }
            catch (Exception ex)
            {
                return false; // Return failure in case of an exception
            }

        }


        public async Task<bool> CheckRegisteredAsync(int childId)
        {
            return await _context.CourseEnrollments.AnyAsync(e => e.ChildID == childId && (e.Status == "Registered" || e.Status == "Confirmed" || e.Status =="Scheduled"|| e.Status =="Completed" ))
                
                || await _context.ActivityEnrollments.AnyAsync(e => e.ChildID == childId && (e.Status == "Registered" || e.Status == "Confirmed" || e.Status == "Canceled"));
        }

        public async Task<bool> CheckPaidAsync(int childId)
        {
            return await _context.Payments.AnyAsync(p => p.ChildID == childId);
        }


    }
}
