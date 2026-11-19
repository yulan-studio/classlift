using System.ComponentModel.DataAnnotations;

namespace Core.FormModels;

public class AddAdminFormModel
{
    [Required(ErrorMessage = "Please enter the administrator's name.")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Please enter an email address.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Wechat { get; set; }

    [Required(ErrorMessage = "Please enter a password.")]
    public string? Password { get; set; } = "hello123!";
}
