using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IUserService
    {

        /// <summary>
        /// Adds a new admin user with the specified email and password.
        /// </summary>
        //Task<bool> AddAdminAsync(string email, string password);

        /// <summary>
        /// Registers a new user with the specified email and password.
        /// </summary>
        //Task<bool> RegisterAsync(string email, string password);

        /// <summary>
        /// Authenticates a user and returns a token or session ID if successful.
        /// </summary>
        //Task<string?> LoginAsync(string email, string password);

        /// <summary>
        /// Changes the password for a user with the specified user ID.
        /// </summary>
        //Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);

        /// <summary>
        /// Retrieves the profile information for a specific user by their ID.
        /// </summary>
        Task<User?> GetUserProfileAsync(int userId);


        Task<IEnumerable<User>> GetAllAsync();
        



    }


}
