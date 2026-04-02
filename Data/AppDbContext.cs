using MAUI_app.Model;
using Microsoft.EntityFrameworkCore;

namespace MAUI_app.Data;

public class AppDbContext : DbContext
{
    public DbSet<ApplicationUser> Users { get; set; }
    public DbSet<Appointment> Appointments { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(Secrets.DatabaseConnection);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.HasIndex(u => u.UserName).IsUnique();
            entity.HasIndex(u => u.Email).IsUnique();

            entity.Property(u => u.Id)
                .UseIdentityByDefaultColumn();
        });
        
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(a => a.Id);

            entity.Property(a => a.Id)
                .UseIdentityByDefaultColumn();

            entity.HasIndex(a => a.AppointmentDate)
                .IsUnique();
        });
    }
}