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

    public class StaffRepository : IStaffRepository
    {
        




        private readonly AppDbContext _context;

        // Constructor to inject DbContext
        public StaffRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Staff> GetByEmailAsync(string email)
        {
            try
            {
                //return await _context.Staff
                //.FirstOrDefaultAsync(u => u.Email == email);
                _context.Staff
                    .FirstOrDefaultAsync(u => u.User.Email == email);
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        // Add a new User to the database asynchronously
        public async Task<bool> AddAsync(Staff entity)
        {
            try
            {
                await _context.Staff.AddAsync(entity);
                await _context.SaveChangesAsync();  // Commit the changes asynchronously
                return true;
            }
            catch (Exception ex)
            {
                return false; // Return failure in case of an exception
            }
        }

        // Remove a User from the database asynchronously
        public async Task<bool> RemoveAsync(Staff entity)
        {
            try
            {
                _context.Staff.Remove(entity);
                await _context.SaveChangesAsync();  // Commit the changes asynchronously
                return true;
            }
            catch (Exception ex)
            {
                return false; // Return failure in case of an exception
            }
           
        }

        // Update an existing User in the database asynchronously
        public async Task<bool> UpdateAsync(Staff entity)
        {
            try
            {
                _context.Entry(entity.User).State = EntityState.Modified;
                _context.Staff.Update(entity);
                await _context.SaveChangesAsync();  // Commit the changes asynchronously
                return true;
            }
            catch (Exception ex)
            {
                return false; // Return failure in case of an exception
            }
        }

        // Find a User by its email asynchronously
        public async Task<Staff> GetAsync(int staffId)
        {
            //return await _context.Staff.FirstAsync(s => s.UserID == userId);
            // return await _context.Staff.FirstOrDefaultAsync(s => s.User.UserID == userId);
            return await _context.Staff.Include(s => s.User)
                 .FirstOrDefaultAsync(s => s.StaffID == staffId);
        }

        // Get all Users from the database asynchronously
        public async Task<IEnumerable<Staff>> GetAllAsync()
        {
            return await _context.Staff
                .Include(s=> s.User)
                .ToListAsync();  // Retrieves all users asynchronously
        }








    }








}
