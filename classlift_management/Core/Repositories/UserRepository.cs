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

    public class UserRepository : IUserRepository<User>
    {
        




        private readonly AppDbContext _context;

        // Constructor to inject DbContext
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            try
            {
                return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        // Add a new User to the database asynchronously
        public async Task<bool> AddAsync(User entity)
        {
            try
            {
                await _context.Users.AddAsync(entity);
                await _context.SaveChangesAsync();  // Commit the changes asynchronously
                return true;
            }
            catch (Exception ex)
            {
                return false; // Return failure in case of an exception
            }
        }

        // Remove a User from the database asynchronously
        public async Task<bool> RemoveAsync(User entity)
        {
            try
            {
                _context.Users.Remove(entity);
                await _context.SaveChangesAsync();  // Commit the changes asynchronously
                return true;
            }
            catch (Exception ex)
            {
                return false; // Return failure in case of an exception
            }
           
        }

        // Update an existing User in the database asynchronously
        public async Task<bool> UpdateAsync(User entity)
        {
            try
            {
                _context.Users.Update(entity);
                await _context.SaveChangesAsync();  // Commit the changes asynchronously
                return true;
            }
            catch (Exception ex)
            {
                return false; // Return failure in case of an exception
            }
        }

        // Find a User by its email asynchronously
        public async Task<User> GetAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);  // Finds by ID asynchronously
        }

        // Get all Users from the database asynchronously
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();  // Retrieves all users asynchronously
        }








    }








}
