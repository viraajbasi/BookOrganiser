using Microsoft.AspNetCore.Identity;

namespace BookOrganiser.Models;

public class UserAccount : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public List<Book> Books { get; set; } = new List<Book>();
}