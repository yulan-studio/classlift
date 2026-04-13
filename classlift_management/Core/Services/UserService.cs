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
    public class JwtOptions
    {
        public string SecretKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int TokenExpirationMinutes { get; set; }
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository<User> _userRepository;
        //private readonly IPasswordHasher<User> _passwordHasher;
        //private readonly JwtOptions _jwtOptions;
        //private const int TokenExpirationMinutes = 60; // Token validity duration



        public UserService(IUserRepository<User> userRepository)
        {
            _userRepository = userRepository;
            //_passwordHasher = password;
            //_jwtOptions = jwtOptions.Value;

        }

      



        //public async Task<bool> RegisterAsync(string email, string password)
        //{
          
        //    if (await _userRepository.GetByEmailAsync(email) != null)
        //            return false; // User already exists

        //    var newUser = new User
        //    {
        //        Email = email,
        //        Role = "Child",
        //        PasswordHash = password,
        //        CreatedDate = DateTime.UtcNow,
                
        //    };

        //    newUser.PasswordHash = _passwordHasher.HashPassword(newUser, password);
        //    await _userRepository.AddAsync(newUser);
        //    return true;
        //}

 


        //public async Task<string?> LoginAsync(string email, string password)
        //{
        //    var user = await _userRepository.GetByEmailAsync(email);
        //    if (user == null || _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password) == PasswordVerificationResult.Failed)
        //        return null; // Invalid username or password

        //    return GenerateToken(user); 
        //}

        //public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        //{
        //    var user = await _userRepository.GetAsync(userId);
        //    if (user == null || _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, currentPassword) == PasswordVerificationResult.Failed)
        //        return false; // User not found or incorrect password

        //    user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
        //    user.UpdatedDate = DateTime.UtcNow;
        //    await _userRepository.UpdateAsync(user);
        //    return true;
        //}

        public async Task<User?> GetUserProfileAsync(int userId)
        {
            return await _userRepository.GetAsync(userId);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            try
            {
                // Fetch all staff records from the repository
                var userList = await _userRepository.GetAllAsync();

                // You can add additional logic or transformations here if necessary
                return userList;
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed (e.g., logging)
                throw new Exception("An error occurred while retrieving staff records.", ex);
            }
        }

        //private string GenerateToken(User user)
        //{
        //    // This method should implement token generation (e.g., JWT)
        //    // Define the key and signing credentials
        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
        //    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        //    List<string> roleList = new List<string> { "Admin", "Staff", "Coach#", "Child" };

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





