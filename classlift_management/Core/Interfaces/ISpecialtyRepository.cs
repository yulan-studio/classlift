
using Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ISpecialtyRepository
    {
        Task<bool> AddAsync(Specialty entity);


        // Remove a Specialty
        Task<bool> RemoveAsync(Specialty entity);


        // Update a Specialty
        Task<bool> UpdateAsync(Specialty entity);


        // Get a Specialty by ID
        Task<Specialty> GetAsync(int id);

        // Get all Specialties
        Task<IEnumerable<Specialty>> GetAllAsync();

        // Get all Specialties
        Task<IEnumerable<Specialty>> GetByNameAsync(string name);
       


    }



}
