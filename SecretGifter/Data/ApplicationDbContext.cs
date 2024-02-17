using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SecretGifter.Models;

namespace SecretGifter.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Group> Groups { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<GroupUser> GroupUser { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<GroupUser>()
            .HasKey(gu => new { gu.UserId, gu.GroupId });

        modelBuilder.Entity<Group>()
            .HasMany(g => g.GroupUsers)
            .WithOne(gu => gu.Group)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ApplicationUser>()
            .HasMany(u => u.GroupUsers)
            .WithOne(gu => gu.User)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
