using MAUI_app.Model;
using MAUI_app.Services;
using Microsoft.EntityFrameworkCore;

namespace MAUI_app.Data;

public class AppDbContext : DbContext
{
    public DbSet<ApplicationUser> Users { get; set; }
    public DbSet<Appointment> Appointments { get; set; }

    public AppDbContext() 
    { 
    }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
    { 
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql(Secrets.DatabaseConnection);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>(
                        v => DateTime.SpecifyKind(v, DateTimeKind.Unspecified), 
            
                        v => DateTime.SpecifyKind(v, DateTimeKind.Unspecified)));          
                }
            }
        }
        
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.HasIndex(u => u.UserName).IsUnique();
            entity.HasIndex(u => u.Email).IsUnique();

            entity.Property(u => u.Id)
                .UseIdentityByDefaultColumn();
            
            
            entity.HasData(new ApplicationUser
            {
                Id = 1,
                UserName = "admin",
                HashedPassword = PasswordHasher.HashPassword("admin"),
                Role = UserRole.Doctor
            });
        });
        
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(a => a.Id);

            entity.Property(a => a.Id)
                .UseIdentityByDefaultColumn();

            entity.HasIndex(a => new { a.DoctorId, a.AppointmentDate })
                .IsUnique();
        });
    }
}