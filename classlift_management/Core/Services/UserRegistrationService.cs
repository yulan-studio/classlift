using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models;
using Core.Interfaces;
using Microsoft.AspNetCore.Identity;



namespace Core.Services
{
    
    public class UserRegistrationService : IUserRegistrationService
    {
        private readonly UserManager<User> _userManager;

        public UserRegistrationService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> RegisterUserAsync(string email, string password, string role, User user)
        {
            var newUser = new User
            {
                UserName = email,
                Email = email,
                Role = role,
                CreatedBy = user.Id
                //CreatedDate = DateTime.UtcNow
                
            };

            try
            {
                var result = await _userManager.CreateAsync(newUser, password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(newUser, role);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw;
            }

           
            
        }


    }
}





