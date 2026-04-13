using Core.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ISpecialtyService
    {
        Task<bool> AddAsync(Specialty specialty);

        // ✅ Update a Specialty
        Task<bool> UpdateAsync(Specialty specialty);


        // ✅ Delete a Specialty
        Task<bool> DeleteAsync(int specialtyId);

        // ✅ Get Specialty by ID
        Task<Specialty> GetAsync(int specialtyId);


        // ✅ Get All Specialty
        Task<IEnumerable<Specialty>> GetAllAsync();


    }


}
