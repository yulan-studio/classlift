
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Contexts;
using Core.Models;
//using Core.mo

namespace Core.Interfaces
{
    public interface ICoachRepository
    {
        Task<Coach> GetByEmailAsync(string email);

       



        // Add a new User to the database asynchronously
        Task<bool> AddAsync(Coach entity);


        // Remove a User from the database asynchronously
       Task<bool> RemoveAsync(Coach entity);
       


        // Update an existing User in the database asynchronously
        Task<bool> UpdateAsync(Coach entity);


        // Find a User by its email asynchronously
        Task<Coach> GetAsync(int coachId);

        //Task<Coach> GetByCoachIdAsync(int coachId);
        Task<Coach?> GetCoachByIdAsync(int coachId);

        // Get all Users from the database asynchronously
        Task<IEnumerable<Coach>> GetAllAsync();

        Task<IEnumerable<Coach>> GetCoachesBySpecialtyAsync(int specialtyId);

       // Task<IEnumerable<Course>> GetActiveCoursesByCoachAsync(int coachId);

    }
}
