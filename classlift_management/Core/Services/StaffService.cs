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
    

    public class StaffService : IStaffService
    {
        private readonly IStaffRepository _staffRepository;
        private readonly IUserRegistrationService _userRegistrationService;
        private readonly IUserRepository<User> _userRepository;
        private readonly UserManager<Core.Models.User> _userManager;
        //private readonly IRepository<Specialty> _specialtyRepository;
        //private readonly IRepository<City> _cityRepository;
        //private readonly IPasswordHasher<Staff> _passwordHasher;
        //private readonly JwtOptions _jwtOptions;
        //private const int TokenExpirationMinutes = 60; // Token validity duration



        public StaffService(IStaffRepository staffRepository, IUserRegistrationService userRegistrationService, UserManager<User> userManager, IUserRepository<User> userRepository)
        {
            _staffRepository = staffRepository;
            _userRegistrationService = userRegistrationService;
            _userManager = userManager;
            _userRepository = userRepository;
            //_passwordHasher = password;
            //_jwtOptions = jwtOptions.Value;

        }

        public async Task<bool> AddAsync(string name, string email, string password, string phone, string wechat, User user)
        {
            // Check if a user with the same username or email already exists
            var existingUser = await _staffRepository.GetByEmailAsync(email);
            if (existingUser != null)
            {
                throw new Exception("A staff with the same username or email already exists.");
            }

            var result = await _userRegistrationService.RegisterUserAsync(email, password, "Staff", user);

            if (result == true)
            {
                //var user = await _userRepository.GetByEmailAsync(email);
                var newUser = await _userManager.FindByEmailAsync(email);
                if (newUser != null)
                {
                    var staffUser = new Staff
                    {
                        UserID = newUser.Id,
                        Name = name,
                        Phone = phone,
                        Wechat = wechat,


                    };

                    
                    return await _staffRepository.AddAsync(staffUser);
                }
                else return false;
            }
            else
                return false;


        }



        public async Task<bool> RemoveAsync(int staffId)
        {
            // Find the staff by ID
            var staff = await _staffRepository.GetAsync(staffId);
            if (staff == null)
            {
                throw new Exception("Staff not found.");
            }

            // Remove the staff
            var result = await _staffRepository.RemoveAsync(staff);

           
            if (result)
                result = await _userRepository.RemoveAsync(staff.User);
            return result;

        }


        public async Task<bool> UpdateAsync(int staffId, string name,string email, /*string password,*/ string phone, string wechat, User user)
        {
            // Find the staff by ID
            var staff = await _staffRepository.GetAsync(staffId);
            if (staff == null)
            {
                throw new Exception("Staff not found.");
            }

            // Update fields
            staff.Name = name;
            staff.User.Email = email;
            staff.Phone = phone;
            staff.Wechat = wechat;
            staff.User.UpdatedBy = user.Id;
            //staff.UpdatedDate = DateTime.UtcNow;

            // Update the password if provided
            //if (!string.IsNullOrWhiteSpace(password))
            //{
            //    staff.Password = _passwordHasher.HashPassword(staff, password);
            //}

            // Save changes
            return await _staffRepository.UpdateAsync(staff);
        }

        public async Task<Staff> GetAsync(int staffId)
        {
            // Retrieve the staff by ID
            var staff = await _staffRepository.GetAsync(staffId);
            if (staff == null)
            {
                throw new Exception("Staff not found.");
            }

            return staff;
        }




        public async Task<IEnumerable<Staff>> GetAllAsync()
        {
            try
            {
                // Fetch all staff records from the repository
                var staffList = await _staffRepository.GetAllAsync();

                // You can add additional logic or transformations here if necessary
                return staffList;
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed (e.g., logging)
                throw new Exception("An error occurred while retrieving staff records.", ex);
            }
        }



        //public async Task<bool> RegisterAsync(string name, string email, string password, int specialtyId, string gender, string phone, string wechat, int cityId)
        //{
        //    return await AddAsync(name, email, password, phone, wechat);
        //}

 


        //public async Task<string?> LoginAsync(string email, string password)
        //{
        //    var user = await _staffRepository.GetByEmailAsync(email);
        //    if (user == null || _passwordHasher.VerifyHashedPassword(user, user.Password, password) == PasswordVerificationResult.Failed)
        //        return null; // Invalid username or password

        //    return GenerateToken(user); 
        //}

        //public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        //{
        //    var user = await _staffRepository.GetAsync(userId);
        //    if (user == null || _passwordHasher.VerifyHashedPassword(user, user.Password, currentPassword) == PasswordVerificationResult.Failed)
        //        return false; // User not found or incorrect password

        //    user.Password = _passwordHasher.HashPassword(user, newPassword);
        //    user.UpdatedDate = DateTime.UtcNow;
        //    await _staffRepository.UpdateAsync(user);
        //    return true;
        //}

        //public async Task<User?> GetUserProfileAsync(int userId)
        //{
        //    return await _staffRepository.GetAsync(userId);
        //}

        

        //private string GenerateToken(User user)
        //{
        //    // This method should implement token generation (e.g., JWT)
        //    // Define the key and signing credentials
        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
        //    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        //    List<string> roleList = new List<string> { "Admin", "Staff", "Coach", "Child" };

        //// Define claims
        //var claims = new List<Claim>
        //{
        //    //new Claim(JwtRegisteredClaimNames.Sub, user.Username), // Subject
        //    new Claim(JwtRegisteredClaimNames.Email, user.Email),  // Email
        //    new Claim("role", user.Role),                         // Custom claim for role
        //    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Unique identifier
        //};

        //    // Create the token
        //    var token = new JwtSecurityToken(
        //        issuer: _jwtOptions.Issuer,          // Replace with your issuer (e.g., your app name)
        //        audience: _jwtOptions.Audience,      // Replace with your audience
        //        claims: claims,
        //        expires: DateTime.UtcNow.AddMinutes(_jwtOptions.TokenExpirationMinutes),
        //        signingCredentials: credentials
        //    );

        //    // Return the serialized token
        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}
    }
}





