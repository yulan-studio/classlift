using System.ComponentModel.DataAnnotations;

namespace Core.ViewModels;

public sealed class OrganizationEmailSettingsViewModel
{
    [Required(ErrorMessage = "Please enter the address that should receive replies to organization emails.")]
    [EmailAddress(ErrorMessage = "Enter a valid reply-to email address.")]
    [StringLength(254)]
    [Display(Name = "Reply-to email address")]
    public string ReplyToEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter the email address that receives organization notifications.")]
    [EmailAddress(ErrorMessage = "Enter a valid notification email address.")]
    [StringLength(254)]
    [Display(Name = "Notification recipient email address")]
    public string ReceiverEmail { get; set; } = string.Empty;
}
