using System.ComponentModel.DataAnnotations;

namespace Core.ViewModels;

public sealed class ResetPasswordViewModel
{
    [Required]
    [Display(Name = "Username")]
    public string SearchUsername { get; set; } = string.Empty;

    public int? TargetUserId { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Role { get; set; }
    public string? DisplayName { get; set; }

    public bool HasUserProfile => TargetUserId.HasValue;
}
