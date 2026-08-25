using System.ComponentModel.DataAnnotations;

namespace Core.ViewModels
{
    public class HomePageSettingsViewModel
    {
        [Required(ErrorMessage = "Please enter the page URL.")]
        [StringLength(2048)]
        [Display(Name = "Page URL")]
        public string PageUrl { get; set; } = "https://courses.roboturtle.ca/";
    }
}
