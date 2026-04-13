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

    public class AdminRepository : IAdminRepository
    {
        




        private readonly AppDbContext _context;

        // Constructor to inject DbContext
        public AdminRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Admin> GetByEmailAsync(string email)
        {
            try
            {
                return await _context.Admins
                .Include(u => u.User)
                .FirstOrDefaultAsync(u => u.User.Email == email);
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        // Add a new User to the database asynchronously
        public async Task<bool> AddAsync(Admin entity)
        {
            try
            {
                await _context.Admins.AddAsync(entity);
                await _context.SaveChangesAsync();  // Commit the changes asynchronously
                return true;
            }
            catch (Exception ex)
            {
                return false; // Return failure in case of an exception
            }
        }

        // Remove a User from the database asynchronously
        public async Task<bool> RemoveAsync(Admin entity)
        {
            try
            {
                _context.Admins.Remove(entity);
                await _context.SaveChangesAsync();  // Commit the changes asynchronously
                return true;
            }
            catch (Exception ex)
            {
                return false; // Return failure in case of an exception
            }
           
        }

        // Update an existing User in the database asynchronously
        public async Task<bool> UpdateAsync(Admin entity)
        {
            try
            {
                _context.Admins.Update(entity);
                await _context.SaveChangesAsync();  // Commit the changes asynchronously
                return true;
            }
            catch (Exception ex)
            {
                return false; // Return failure in case of an exception
            }
        }

        // Find a User by its email asynchronously
        public async Task<Admin> GetAsync(int adminId)
        {
            return await _context.Admins.Include(u=>u.User)
                .FirstOrDefaultAsync(u=> u.AdminID == adminId);  // Finds by ID asynchronously
        }

        // Get all Users from the database asynchronously
        public async Task<IEnumerable<Admin>> GetAllAsync()
        {
            return await _context.Admins.Include(u=>u.User)
            
                .ToListAsync();  
        }








    }








}
