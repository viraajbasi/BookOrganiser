using BookOrganiser.Models;
using Microsoft.EntityFrameworkCore;

namespace BookOrganiser.Data;

public class AppDbContext: DbContext
{
    public DbSet<Book> Books { get; set; }

    public AppDbContext(DbContextOptions options): base(options) {   }
}