using System.ComponentModel.DataAnnotations;

namespace Core.ViewModels;

public sealed class TimeZonePreferenceViewModel
{
    [Required]
    [StringLength(100)]
    public string TimeZoneId { get; set; } = string.Empty;
    public string? ReturnUrl { get; set; }
}
