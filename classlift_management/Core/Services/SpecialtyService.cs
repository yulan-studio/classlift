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
    public class SpecialtyService: ISpecialtyService
    {
        private readonly ISpecialtyRepository _specialtyRepository;

        public SpecialtyService(ISpecialtyRepository specialtyRepository)
        {
            _specialtyRepository = specialtyRepository;
        }

        // ✅ Add a City
        public async Task<bool> AddAsync(Specialty specialty)
        {
            if (string.IsNullOrWhiteSpace(specialty.Title))
                throw new ArgumentException("Specialty name cannot be empty.");
            var existingSpecialty = await _specialtyRepository.GetByNameAsync(specialty.Title);
            if (existingSpecialty.Any())
                throw new InvalidOperationException("This specialty already exist.");
            return await _specialtyRepository.AddAsync(specialty);
        }

        // ✅ Update a City
        public async Task<bool> UpdateAsync(Specialty specialty)
        {
            var existingSpecialty = await _specialtyRepository.GetAsync(specialty.SpecialtyID);
            if (existingSpecialty == null)
                throw new KeyNotFoundException("Specialty not found.");

            existingSpecialty.Title = specialty.Title; // Update fields
            existingSpecialty.Description = specialty.Description;
            return await _specialtyRepository.UpdateAsync(existingSpecialty);
        }

        // ✅ Delete a City
        public async Task<bool> DeleteAsync(int specialtyId)
        {
            var specialty = await _specialtyRepository.GetAsync(specialtyId);
            if (specialty == null)
                throw new KeyNotFoundException("Specialty not found.");

            return await _specialtyRepository.RemoveAsync(specialty);
        }

        // ✅ Get City by ID
        public async Task<Specialty> GetAsync(int specialtyId)
        {
            return await _specialtyRepository.GetAsync(specialtyId) ?? throw new KeyNotFoundException("Specialty not found.");
        }

       

        // ✅ Get All Cities
        public async Task<IEnumerable<Specialty>> GetAllAsync()
        {
            try
            {
                // Fetch all coach records from the repository
                var specialtyList = await _specialtyRepository.GetAllAsync();

                // You can add additional logic or transformations here if necessary
                return specialtyList.OrderBy(s=>s.Title);
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed (e.g., logging)
                throw new Exception("An error occurred while retrieving specialty records.", ex);
            }

          
        }
    }
}
