using Microsoft.AspNetCore.Http;

namespace Core.ViewModels
{
    public class BrandingSettingsViewModel
    {
        public IFormFile? Logo { get; set; }

        public string CurrentLogoUrl { get; set; } = "/images/classlift_logo.png";
    }
}
