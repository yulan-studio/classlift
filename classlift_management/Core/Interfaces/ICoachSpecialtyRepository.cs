
using Core.Models;
using Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ICoachSpecialtyRepository
    {
        //Task<bool> AddAsync(CoachSpecialty entity);

        //Task<IEnumerable<int>> GetSpecialtyIdsByCoachAsync(int coachId);


        // Remove a Specialty
        //Task<bool> RemoveAsync(City entity);


        //// Update a Specialty
        //Task<bool> UpdateAsync(City entity);


        //// Get a Specialty by ID
        //Task<City> GetAsync(int id);

        //// Get all Specialties
        //Task<IEnumerable<City>> GetAllAsync();

        //// Get all Specialties
        //Task<IEnumerable<City>> GetByNameAsync(string name);

        Task<List<Specialty>> GetSpecialtiesByCoachAsync(int coachId);

        Task<List<Coach>> GetCoachesBySpecialtyAsync(int specialtyId);





    }



}
