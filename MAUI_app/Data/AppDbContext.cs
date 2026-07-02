using System;
using MAUI_app.Model;
using MAUI_app.Services;
using Microsoft.EntityFrameworkCore;

namespace MAUI_app.Data;

public class AppDbContext : DbContext
{
    public DbSet<ApplicationUser> Users { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Medication> Medications { get; set; }
    
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
        
        optionsBuilder.ConfigureWarnings(warnings => 
            warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
    }
    
    private static readonly Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime> DateTimeConverter = 
        new(v => DateTime.SpecifyKind(v, DateTimeKind.Unspecified), 
            v => DateTime.SpecifyKind(v, DateTimeKind.Unspecified));
            
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(DateTimeConverter);          
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
                Email = "admin@admin.com",
                HashedPassword = "$2a$11$yp4bvraezYGvSzDFdH48luRXhKDhT60bGU9HG5bh01BsVRdnsMnpe",
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
                
            entity.HasIndex(a => a.ApplicationUserId);
        });
        
        modelBuilder.Entity<Medication>(entity =>
        {
            entity.HasKey(m => m.Id);

            entity.HasIndex(m => m.ApplicationUserId);
            entity.HasIndex(m => m.DoctorId);
            entity.HasIndex(m => m.StartDate);
            entity.HasIndex(m => m.EndDate);
        });
    }
}