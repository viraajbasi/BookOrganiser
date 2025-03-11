using System.ComponentModel.DataAnnotations;

namespace BookOrganiser.ViewModels.Account;

public class ChangePasswordViewModel
{
    [Required(ErrorMessage = "An email is required.")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A password is required.")]
    [StringLength(40, MinimumLength = 8, ErrorMessage = "The {0} must be at least {2} and at most {0} characters long.")]
    [DataType(DataType.Password)]
    [Compare("ConfirmNewPassword", ErrorMessage = "Passwords don't match.")]
    [Display(Name = "New Password")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirm your password.")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm New Password")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}