using BookOrganiser.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace BookOrganiser.Data;

public class AppDbContext: IdentityDbContext<UserAccount>
{
    public DbSet<Book> Books { get; set; }
    public DbSet<UserAccount> UserAccounts { get; set; }
    public DbSet<AISummary> AISummary { get; set; }

    public AppDbContext(DbContextOptions options): base(options) {   }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserAccount>()
            .HasMany(e => e.Books)
            .WithOne(e=> e.UserAccount)
            .HasForeignKey(e => e.UserId)
            .IsRequired();
        
        modelBuilder.Entity<Book>()
            .HasOne(e => e.AISummary)
            .WithOne(e => e.Book)
            .HasForeignKey<AISummary>(e => e.BookId)
            .IsRequired();
        
        base.OnModelCreating(modelBuilder);
    }
}