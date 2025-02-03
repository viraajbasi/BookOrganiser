using Microsoft.AspNetCore.Identity;

namespace BookOrganiser.Models;

public class UserAccount : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public IList<Book> Books { get; set; } = new List<Book>();
    public IList<string> UserCategories { get; set; } = new List<string>();
}