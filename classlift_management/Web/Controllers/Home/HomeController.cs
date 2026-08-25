using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Core.Models;
using Core.R2;

namespace Web.Controllers.Home
{
    [Authorize]
    public class HomeController : Controller
    {
        private const string DefaultHomePageUrl = "https://courses.roboturtle.ca/";
        private readonly R2StorageService _storageService;
        private readonly CurrentTenant _currentTenant;

        public HomeController(
            R2StorageService storageService,
            CurrentTenant currentTenant)
        {
            _storageService = storageService;
            _currentTenant = currentTenant;
        }

        public async Task<IActionResult> Index()
        {
            var pageUrl = DefaultHomePageUrl;

            if (!string.IsNullOrWhiteSpace(_currentTenant.DatabaseName))
            {
                var savedUrl = await _storageService.GetTextAsync(
                    $"branding/{_currentTenant.DatabaseName}/home-page-url.txt");

                if (Uri.TryCreate(savedUrl?.Trim(), UriKind.Absolute, out var savedUri)
                    && (savedUri.Scheme == Uri.UriSchemeHttps || savedUri.Scheme == Uri.UriSchemeHttp))
                {
                    pageUrl = savedUri.AbsoluteUri;
                }
            }

            ViewBag.HomePageUrl = pageUrl;
            return View();
        }
    }
}
