using System.ComponentModel.DataAnnotations;

namespace Core.ViewModels;

public sealed class CoachContactSettingsViewModel
{
    [Display(Name = "Chosen Name")]
    [MaxLength(255)]
    public string? PreferedName { get; set; }

    [MaxLength(100)]
    [Display(Name = "WeChat")]
    public string? Wechat { get; set; }

    [MaxLength(50)]
    [Display(Name = "WhatsApp")]
    public string? WhatsApp { get; set; }

    [Required]
    [Display(Name = "City")]
    public int CityID { get; set; }

    [MaxLength(100)]
    public string? Address { get; set; }

    [MaxLength(6)]
    public string? PostCode { get; set; }

    [Required]
    public string Status { get; set; } = "Active";

    [Display(Name = "Photo Consent")]
    public bool PhotoConsent { get; set; }
}
