using System;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models;

namespace Core.Interfaces
{
    public interface IAdminRepository 
    {
        Task<Admin> GetByEmailAsync(string email);



        // Add a new User to the database asynchronously
        Task<bool> AddAsync(Admin entity);


        // Remove a User from the database asynchronously
        Task<bool> RemoveAsync(Admin entity);


        // Update an existing User in the database asynchronously
        Task<bool> UpdateAsync(Admin entity);


        // Find a User by its email asynchronously
        Task<Admin> GetAsync(int adminId);


        // Get all Users from the database asynchronously
        Task<IEnumerable<Admin>> GetAllAsync();


    }
}
