using MAUI_app.Services;
using Microsoft.EntityFrameworkCore;

namespace MAUI_app.Model;

public class AppDbContext : DbContext
{
    public DbSet<ApplicationUser> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=100.119.240.79;Port=5433;Database=uni_db;User Id=postgres;Password=team_db_pass;Trust Server Certificate=true");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.HasIndex(u => u.UserName).IsUnique();
            entity.HasIndex(u => u.Email).IsUnique();
        });
        
    }
}