using BookOrganiser.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace BookOrganiser.Data;

public class AppDbContext: IdentityDbContext<UserAccount>
{
    public DbSet<Book> Books { get; set; }
    public DbSet<UserAccount> UserAccounts { get; set; }

    public AppDbContext(DbContextOptions options): base(options) {   }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserAccount>()
            .HasMany(e => e.Books)
            .WithOne(e=> e.UserAccount)
            .HasForeignKey(e => e.UserId)
            .IsRequired();
        
        base.OnModelCreating(modelBuilder);
    }
}