using System.ComponentModel.DataAnnotations;

namespace BookOrganiser.ViewModels.AccountViewModels;

public class ChangePasswordViewModel
{
    [EmailAddress]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Your current password is required.")]
    [DataType(DataType.Password)]
    [Display(Name = "Current Password")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "A password is required.")]
    [StringLength(40, MinimumLength = 8,
        ErrorMessage = "The password must be between 8 and 40 characters long.")]
    [DataType(DataType.Password)]
    [Compare("ConfirmNewPassword", ErrorMessage = "Passwords don't match.")]
    [Display(Name = "New Password")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirm your password.")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm New Password")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}