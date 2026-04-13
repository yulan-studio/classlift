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
    public class CoachRepository : ICoachRepository
    {
        
        private readonly AppDbContext _context;

        // Constructor to inject DbContext
        public CoachRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Coach> GetByEmailAsync(string email)
        {
            try
            {
                return await _context.Coaches.Include(u => u.User)
                .FirstOrDefaultAsync(u => u.User.Email == email);
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        // Add a new User to the database asynchronously
        public async Task<bool> AddAsync(Coach entity)
        {
            try
            {
                await _context.Coaches.AddAsync(entity);
                await _context.SaveChangesAsync();  // Commit the changes asynchronously
                return true;
            }
            catch (Exception ex)
            {
                return false; // Return failure in case of an exception
            }
        }

        // Remove a User from the database asynchronously
        public async Task<bool> RemoveAsync(Coach entity)
        {
            try
            {
                _context.Coaches.Remove(entity);
                await _context.SaveChangesAsync();  // Commit the changes asynchronously
                return true;
            }
            catch (Exception ex)
            {
                return false; // Return failure in case of an exception
            }
           
        }

        // Update an existing User in the database asynchronously
        public async Task<bool> UpdateAsync(Coach entity)
        {
            try
            {
                _context.Entry(entity.User).State = EntityState.Modified;
                _context.Coaches.Update(entity);
                await _context.SaveChangesAsync();  // Commit the changes asynchronously
                return true;
            }
            catch (Exception ex)
            {
                return false; // Return failure in case of an exception
            }
        }

        // Find a User by its email asynchronously
        public async Task<Coach> GetAsync(int coachId)
        {
            return await _context.Coaches
                .Include(c => c.User)
                .Include(c => c.City)
                .Include(c=> c.CoachSpecialties)
                .ThenInclude(cs => cs.Specialty)
                .Include(c => c.EmergencyContacts)
                .FirstOrDefaultAsync(c => c.CoachID == coachId);
               
            
        }

        public async Task<Coach?> GetCoachByIdAsync(int userId)
        {
            return await _context.Coaches
                .Include(c => c.City)
                .Include(c => c.CoachSpecialties)
                .FirstOrDefaultAsync(c => c.UserID == userId);
        }


        // Get all Users from the database asynchronously
        public async Task<IEnumerable<Coach>> GetAllAsync()
        {
            return await _context.Coaches
                .Include(c => c.User)
                .Include(c => c.City) // Eagerly load the City navigation property
                .Include(c => c.CoachSpecialties) // Eagerly load the Specialty navigation property
                .ThenInclude(cs => cs.Specialty) // Load the specialties via the join table
                .ToListAsync();  // Retrieves all users asynchronously


        //    await _context.Coaches
        //.Include(c => c.User)  // Include user for email
        //.Include(c => c.City)  // Include city for city name
        //.Include(c => c.Coach_Specialties) // Include related specialties
        //.ThenInclude(cs => cs.Specialty)
        //.ToListAsync();

        }

        public async Task<IEnumerable<Coach>> GetCoachesBySpecialtyAsync(int specialtyId)
        {
            return await _context.CoachSpecialties
                .Where(cs => cs.SpecialtyID == specialtyId)
                .Include(cs => cs.Coach) // Ensure Coach entity is loaded
                .Select(cs => cs.Coach)  // Extract the Coach entity
                .ToListAsync();
        }



    }








}
