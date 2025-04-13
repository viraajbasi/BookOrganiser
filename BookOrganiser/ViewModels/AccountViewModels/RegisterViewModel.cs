using System.ComponentModel.DataAnnotations;

namespace BookOrganiser.ViewModels.AccountViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "A name is required.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "An email is required.")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A password is required.")]
    [StringLength(40, MinimumLength = 8, ErrorMessage = "The password must be at least 8 characters long.")]
    [DataType(DataType.Password)]
    [Compare("ConfirmPassword", ErrorMessage = "Passwords don't match.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirm your password.")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}