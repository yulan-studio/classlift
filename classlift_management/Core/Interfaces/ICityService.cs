using Core.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ICityService
    {


        Task<bool> AddAsync(City city);


        // ✅ Update a City
        Task<bool> UpdateAsync(City city);


        // ✅ Delete a City
        Task<bool> DeleteAsync(int cityId);

        // ✅ Get City by ID
        Task<City> GetAsync(int cityId);


        // ✅ Get All Cities
        Task<IEnumerable<City>> GetAllAsync();

        Task<IEnumerable<City>> GetAllUsedAsync();




    }


}
