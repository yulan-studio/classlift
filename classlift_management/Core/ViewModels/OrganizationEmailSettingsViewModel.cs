using System.ComponentModel.DataAnnotations;

namespace Core.ViewModels;

public sealed class OrganizationEmailSettingsViewModel
{
    [Required(ErrorMessage = "Please enter the email address used to send organization emails.")]
    [EmailAddress(ErrorMessage = "Enter a valid sender email address.")]
    [StringLength(254)]
    [Display(Name = "Sender email address")]
    public string SenderEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter the email address that receives organization notifications.")]
    [EmailAddress(ErrorMessage = "Enter a valid notification email address.")]
    [StringLength(254)]
    [Display(Name = "Notification recipient email address")]
    public string ReceiverEmail { get; set; } = string.Empty;
}
