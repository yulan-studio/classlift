
using Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Core.Interfaces;
using Core.Models;

using System.Diagnostics;



namespace Web.Controllers.Authentication
{
    [Route("User")]
    //[ApiController]
    public class UserController: Controller
    {
        private readonly IUserService _userService;
        

        public UserController(IUserService userService)
        {
            _userService = userService;
        }



       
    }
}
