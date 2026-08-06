using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
//using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{

    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IUserRegistrationService _userRegistrationService;
        private readonly IUserRepository<User> _userRepository;
        private readonly UserManager<Core.Models.User> _userManager;
        //private readonly IPasswordHasher<Admin> _passwordHasher;
        //private readonly JwtOptions _jwtOptions;
        //private const int TokenExpirationMinutes = 60; // Token validity duration



        public AdminService(IAdminRepository adminRepository, IUserRegistrationService userRegistrationService, IUserRepository<User> userRepository, UserManager<Core.Models.User> userManager/*, IPasswordHasher<Admin> password, IOptions<JwtOptions> jwtOptions*/)
        {
            _adminRepository = adminRepository;
            _userRegistrationService = userRegistrationService;
            _userRepository = userRepository;
            _userManager = userManager;
            //_passwordHasher = password;
            //_jwtOptions = jwtOptions.Value;

        }

        public async Task<bool> AddAsync(string name, string email, string password, string phone, string wechat, User user)
        {
            // Check if a user with the same username or email already exists
            var existingUser = await _adminRepository.GetByEmailAsync(email);
            if (existingUser != null)
            {
                throw new Exception("A admin with the same email already exists.");
            }


            var result = await _userRegistrationService.RegisterUserAsync(email, password, "Admin", user);

            if (result == true)
            {
                //var user = await _userRepository.GetByEmailAsync(email);
                var newUser = await _userManager.FindByEmailAsync(email);
                if (newUser != null)
                {
                    var adminUser = new Admin
                    {
                        UserID = newUser.Id,
                        Name = name,
                        Phone = phone,
                        Wechat = wechat,
                        
                        
                    };

                   // adminUser.User.CreatedBy = user.Id;
                    return await _adminRepository.AddAsync(adminUser);
                }
                else return false;
            }
            else
                return false;

        }



        public async Task<bool> RemoveAsync(int adminId)
        {
            // Find the staff by ID
            var admin = await _adminRepository.GetAsync(adminId);
            if (admin == null)
            {
                throw new Exception("Staff not found.");
            }

            // Remove the admin
            var result = await _adminRepository.RemoveAsync(admin);
            if (result)
                result = await _userRepository.RemoveAsync(admin.User);
            return result;

        }


        public async Task<bool> UpdateAsync(int adminId, string name, string email, /*string password,*/ string phone, string wechat, User user)
        {
            // Find the staff by ID
            var admin = await _adminRepository.GetAsync(adminId);
            if (admin == null)
            {
                throw new Exception("adminm not found.");
            }

            // Update fields
            admin.Name = name;
            admin.User.Email = email;
            admin.Phone = phone;
            admin.Wechat = wechat;
            admin.User.UpdatedDate = DateTime.UtcNow;
            admin.User.UpdatedBy = user.Id; 

           

            // Save changes
            return await _adminRepository.UpdateAsync(admin);
        }

        public async Task<Admin> GetAsync(int adminId)
        {
            // Retrieve the staff by ID
            var admin = await _adminRepository.GetAsync(adminId);
            if (admin == null)
            {
                throw new Exception("Admin not found.");
            }

            return admin;
        }


        public async Task<IEnumerable<Admin>> GetAllAsync()
        {
            try
            {
                // Fetch all staff records from the repository
                var adminList = await _adminRepository.GetAllAsync();

                // You can add additional logic or transformations here if necessary
                return adminList;
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed (e.g., logging)
                throw new Exception("An error occurred while retrieving admin records.", ex);
            }
        }
     
    }

}
