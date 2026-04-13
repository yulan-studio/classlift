
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
    public interface IStaffRepository
    {

        //Task<Admin> GetByEmailAsync(string email);

        // Add a new User to the database asynchronously
        Task<bool> AddAsync(Staff entity);


        // Remove a User from the database asynchronously
       Task<bool> RemoveAsync(Staff entity);


        // Update an existing User in the database asynchronously
        Task<bool> UpdateAsync(Staff entity);


        // Find a User by its email asynchronously
        Task<Staff> GetAsync(int staffId);


        // Get all Users from the database asynchronously
        Task<IEnumerable<Staff>> GetAllAsync();

        Task<Staff> GetByEmailAsync(string email);

    }
}
