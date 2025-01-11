using BookOrganiser.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace BookOrganiser.Data;

public class AppDbContext: IdentityDbContext<User>
{
    public DbSet<Book> Books { get; set; }
    public DbSet<User> Users { get; set; }

    public AppDbContext(DbContextOptions options): base(options) {   }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasMany(e => e.Books)
            .WithOne(e=> e.User)
            .HasForeignKey(e => e.UserId)
            .IsRequired();
        
        base.OnModelCreating(modelBuilder);
    }
}