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
    public class EmergencyContactService: IEmergencyContactService
    {
        private readonly IEmergencyContactRepository _emergencyContactRepository;

        public EmergencyContactService(IEmergencyContactRepository emergencyContactRepository)
        {
            _emergencyContactRepository = emergencyContactRepository;
        }

        public async Task<bool> AddAsync(EmergencyContact contact)
        {
            if (string.IsNullOrWhiteSpace(contact.ContactName))
                throw new ArgumentException("Contact name cannot be empty.");
          
            return await _emergencyContactRepository.AddAsync(contact);
        }

     


        public async Task<bool> DeleteAsync(int contactId)
        {
            var contact = await _emergencyContactRepository.GetAsync(contactId);
            if (contact == null)
                throw new KeyNotFoundException("Contact not found.");

            return await _emergencyContactRepository.RemoveAsync(contact);
        }

  
        public async Task<EmergencyContact> GetAsync(int contactId)
        {
            return await _emergencyContactRepository.GetAsync(contactId) ?? throw new KeyNotFoundException("Contact not found.");
        }

       



    }
}
