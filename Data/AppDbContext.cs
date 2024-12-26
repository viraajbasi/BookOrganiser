using BookOrganiser.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace BookOrganiser.Data;

public class AppDbContext: IdentityDbContext<User>
{
    public DbSet<Book> Books { get; set; }
    public DbSet<User> Users { get; set; }

    public AppDbContext(DbContextOptions options): base(options) {   }
}