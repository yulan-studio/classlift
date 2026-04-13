using Core.Interfaces;
using Core.Models;
using Core.Repositories;
using Core.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Core.Services
{
    public class ChildService : IChildService
    {
       
      
        private readonly IChildRepository _childRepository;
        private readonly ICityRepository _cityRepository;
        private readonly IUserRepository<User> _userRepository;
        private readonly IUserRegistrationService _userRegistrationService;
        private readonly UserManager<Core.Models.User> _userManager;

        public ChildService(IChildRepository childRepository, ICityRepository cityRepository, IUserRepository<User> userRepository, IUserRegistrationService userRegistrationService, UserManager<Core.Models.User> userManager)
        {
            _childRepository = childRepository;
            _cityRepository = cityRepository;
            _userRepository = userRepository;
            _userRegistrationService = userRegistrationService;
            _userManager = userManager;

        }

        public async Task<IEnumerable<Child>> GetAllAsync()
        {
            return await _childRepository.GetAllAsync();
        }

        public async Task<Child> GetAsync(int childId)
        {
            var child = await _childRepository.GetAsync(childId);
            if (child == null)
            {
                throw new KeyNotFoundException($"Child with ID {childId} not found.");
            }
            return child;
        }

        public async Task<Child?> GetByIdAsync(int userId)
        {
            //var user = await _userRepository.GetAsync(userId);
            //if (user == null)
            //{
            //    throw new KeyNotFoundException($"Child not found.");
            //}
            var child = await _childRepository.GetByIdAsync(userId);
            if (child == null)
            {
                throw new KeyNotFoundException($"Child not found.");
            }
            return child;
        }


        public async Task<bool> AddAsync(string name, DateTime? birthDate, string? gender, int? cityId, string email, string password, bool hasOAP, User user)
        {

            // Check if a user with the same username or email already exists
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                throw new Exception("A child with the same email already exists.");
                
            }

            var result = await _userRegistrationService.RegisterUserAsync(email, password, "Child", user);

            if (result == true)
            {
                //var user = await _userRepository.GetByEmailAsync(email);
                var newUser = await _userManager.FindByEmailAsync(email);
                if (newUser != null)
                {
                    City city = null;
                    if (cityId != null)
                        city = await _cityRepository.GetAsync((int)cityId);
                    var childUser = new Child
                    {
                        MemberID = "",
                        Name = name,
                        BirthDate = birthDate,
                        Gender = gender,
                        User = newUser,
                        City = city, 
                        HasOAP = hasOAP,
                        PrimaryDiagnosis = "",
                        Address = ""
                    };


                    return await _childRepository.AddAsync(childUser);
                }
                else return false;
            }
            else
                return false;

            
        }



        

        public async Task<bool> UpdateAsync(Child child)
        {
            //var existingChild = await _childRepository.GetAsync(child.ChildID);
            //if (existingChild == null)
            //    throw new Exception("Child not found.");

            return await _childRepository.UpdateAsync(child);
        }



        public async Task<bool> UpdateAsync(int childId, string name, DateTime birthDate, string gender, int cityId, string email, bool hasOAP/*, string password*/)
        {
            // Find the coach by ID
            var child = await _childRepository.GetAsync(childId);
            if (child == null)
            {
                throw new KeyNotFoundException($"Child with ID {childId} not found.");
            }

            // Update fields
            //child.MemberID = memberID;
            child.Name = name;
            child.User.Email = email;
            child.BirthDate = birthDate;
            child.Gender = gender;
           
            child.CityID = cityId;
            child.HasOAP = hasOAP;
            child.User.UpdatedDate = DateTimeHelper.GetTorontoTime();

            // Update the password if provided
            //if (!string.IsNullOrWhiteSpace(password))
            //{
            //    coach.Password = _passwordHasher.HashPassword(coach, password);
            //}

            // Save changes
            return await _childRepository.UpdateAsync(child);
        }



        public async Task<bool> UpdateAsync(int childId, string? memberID, string? address, /*int OAPAmount,*/  string? primaryDiagnosis, bool photoConsent/*, string password*/)
        {
            // Find the coach by ID
            var child = await _childRepository.GetAsync(childId);
            if (child == null)
            {
                throw new KeyNotFoundException($"Child with ID {childId} not found.");
            }

            // Update fields
            child.MemberID = memberID;
            child.Address = address;
            //child.OAPAmount = OAPAmount;
            child.PrimaryDiagnosis = primaryDiagnosis;
            child.PhotoConsent = photoConsent;
           
           // child.User.UpdatedDate = DateTime.UtcNow;

            // Update the password if provided
            //if (!string.IsNullOrWhiteSpace(password))
            //{
            //    coach.Password = _passwordHasher.HashPassword(coach, password);
            //}

            // Save changes
            return await _childRepository.UpdateAsync(child);
        }

        public async Task<bool> RemoveAsync(int childId)
        {

            // Find the child by ID
            var child = await _childRepository.GetAsync(childId);
            if (child == null)
            {
                throw new KeyNotFoundException($"Child with ID {childId} not found.");
            }

            // Remove the child
            var result = await _childRepository.RemoveAsync(child);


            if (result)
                result = await _userRepository.RemoveAsync(child.User);
            return result;
        }

        public async Task<bool> CheckRegisteredAsync(int childId)
        {
            var result = await _childRepository.CheckRegisteredAsync(childId);
            return result;
        }

        public async Task<bool> CheckPaidAsync(int childId)
        {
            var result = await _childRepository.CheckPaidAsync(childId);
            return result;
        }


        //public async Task<List<ChildCalendarViewModel>> GetChildSchedules(int childId)
        //{
        //    var events = await _childCalendar.GetChildSchedules(childId);

            
        //    return events.OrderBy(e => e.Start).ToList();
        //}

    }
}





