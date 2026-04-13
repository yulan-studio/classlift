using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Core.Models;
using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Core.Repositories;

namespace Core.Services
{
    public class CityService: ICityService
    {
        private readonly ICityRepository _cityRepository;

        public CityService(ICityRepository cityRepository)
        {
            _cityRepository = cityRepository;
        }

        // ✅ Add a City
        public async Task<bool> AddAsync(City city)
        {
            if (string.IsNullOrWhiteSpace(city.Name))
                throw new ArgumentException("City name cannot be empty.");
            var existingCity = await _cityRepository.GetByNameAsync(city.Name);
            if (existingCity.Any())
                throw new InvalidOperationException("This city already exist.");
            return await _cityRepository.AddAsync(city);
        }

        // ✅ Update a City
        public async Task<bool> UpdateAsync(City city)
        {
            var existingCity = await _cityRepository.GetAsync(city.CityID);
            if (existingCity == null)
                throw new KeyNotFoundException("City not found.");

            existingCity.Name = city.Name; // Update fields

            return await _cityRepository.UpdateAsync(existingCity);
        }

        // ✅ Delete a City
        public async Task<bool> DeleteAsync(int cityId)
        {
            var city = await _cityRepository.GetAsync(cityId);
            if (city == null)
                throw new KeyNotFoundException("City not found.");

            return await _cityRepository.RemoveAsync(city);
        }

        // ✅ Get City by ID
        public async Task<City> GetAsync(int cityId)
        {
            return await _cityRepository.GetAsync(cityId) ?? throw new KeyNotFoundException("City not found.");
        }

        // ✅ Get All Cities
        public async Task<IEnumerable<City>> GetAllAsync()
        {
            try
            {
                // Fetch all coach records from the repository
                var cityList = await _cityRepository.GetAllAsync();

                // You can add additional logic or transformations here if necessary
                return cityList.OrderBy(c=> c.Name);
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed (e.g., logging)
                throw new Exception("An error occurred while retrieving city records.", ex);
            }

          
        }


        public async Task<IEnumerable<City>> GetAllUsedAsync()
        {
            try
            {
                
                // Fetch all coach records from the repository
                var cityList = await _cityRepository.GetAllUsedAsync();

                // You can add additional logic or transformations here if necessary
                return cityList;
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed (e.g., logging)
                throw new Exception("An error occurred while retrieving city records.", ex);
            }


        }

    }
}
