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
        base.OnModelCreating(modelBuilder);
        
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>(
                        // When saving TO the database: Convert any local/unspecified time to UTC 
                        v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(), 
                
                        // When reading FROM the database: Explicitly mark the incoming value 
                        // as UTC so C# knows how to handle it correctly without crashing.
                        v => DateTime.SpecifyKind(v, DateTimeKind.Utc)));          
                }
            }
        }
        
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