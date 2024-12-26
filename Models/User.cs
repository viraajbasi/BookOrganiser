using Microsoft.AspNetCore.Identity;

namespace BookOrganiser.Models;

public class User : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}