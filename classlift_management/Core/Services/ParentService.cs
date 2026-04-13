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
using Microsoft.AspNetCore.Http.HttpResults;
using System.Net;

namespace Core.Services
{
    public class ParentService : IParentService
    {
        private readonly IParentRepository _parentRepository;

        public ParentService(IParentRepository parentRepository)
        {
            _parentRepository = parentRepository;
        }

        // ✅ Get all parents
        public async Task<IEnumerable<Parent>> GetAllParentsAsync()
        {
            return await _parentRepository.GetAllAsync();
        }

        // ✅ Get a parent by ID
        public async Task<Parent?> GetParentByIdAsync(int parentId)
        {
            return await _parentRepository.GetAsync(parentId);
        }

       
        // ✅ Add a new parent
        public async Task<bool> AddAsync(Parent parent)
        {
            if (string.IsNullOrWhiteSpace(parent.Name))
                throw new ArgumentException("Parent name cannot be empty.");

            return await _parentRepository.AddAsync(parent);
        }

        public async Task<int> AddAndReturnIdAsync(Parent parent)
        {
            return await _parentRepository.AddAndReturnIdAsync(parent);
        }

        // ✅ Update an existing parent
        public async Task<bool> UpdateAsync(Parent parent)
        {
            var existingParent = await _parentRepository.GetAsync(parent.ParentID);
            if (existingParent == null)
                throw new Exception("Parent not found.");

            return await _parentRepository.UpdateAsync(parent);
        }

        // ✅ Delete a parent
        public async Task<bool> DeleteAsync(int parentId)
        {
            var parent = await _parentRepository.GetAsync(parentId);
            if (parent == null)
                throw new Exception("Parent not found.");

            return await _parentRepository.DeleteAsync(parentId);
        }
    }


}





