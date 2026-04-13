using Core.Interfaces;
using Core.Models;
using Core.Repositories;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.User
{
    public class ParentController : Controller
    {
        private readonly UserManager<Core.Models.User> _userManager;
        private readonly IParentService _parentService;
        private readonly IParentChildService _parentChildService;
        //public IActionResult Index()
        //{
        //    return View();
        //}

        public ParentController(IParentService parentService, IParentChildService parentChildService, UserManager<Core.Models.User> userManager)
        {
            _parentService = parentService;
            _parentChildService = parentChildService;
            _userManager = userManager;
        }

        

       
    }
}
