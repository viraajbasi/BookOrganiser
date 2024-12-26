using System.ComponentModel.DataAnnotations;

namespace BookOrganiser.ViewModels;

public class VerifyEmailViewModel()
{
    [Required(ErrorMessage = "An email is required.")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}