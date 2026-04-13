
using Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ICityRepository
    {
        Task<bool> AddAsync(City entity);


        // Remove a Specialty
        Task<bool> RemoveAsync(City entity);


        // Update a Specialty
        Task<bool> UpdateAsync(City entity);


        // Get a Specialty by ID
        Task<City> GetAsync(int id);

        // Get all Specialties
        Task<IEnumerable<City>> GetAllAsync();


        Task<IEnumerable<City>> GetAllUsedAsync();

        // Get all Specialties
        Task<IEnumerable<City>> GetByNameAsync(string name);
       


    }



}
