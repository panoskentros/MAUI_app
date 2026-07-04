using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MAUI_app.Data;
using MAUI_app.Model;
using MAUI_app.Services;
using MAUI_app.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MAUI_app.Tests.Services;

public class MedicationServiceTests
{
    private AppDbContext GetDatabaseContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var databaseContext = new AppDbContext(options);
        databaseContext.Database.EnsureCreated();
        
        databaseContext.Set<ApplicationUser>().RemoveRange(databaseContext.Set<ApplicationUser>());
        databaseContext.SaveChanges();
        
        return databaseContext;
    }

    [Fact]
    public async Task GetMedicationsAsync_Doctor_ReturnsAll()
    {
        var context = GetDatabaseContext();
        var doctor = new ApplicationUser { Id = 2, UserName = "DoctorOne", Role = UserRole.Doctor, HashedPassword = "dummy" };
        
        context.Users.Add(doctor);
        context.Medications.AddRange(new List<Medication>
        {
            new Medication { Id = 1, MedicationName = "Aspirin", DoctorId = 2, ApplicationUserId = 1, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(7) },
            new Medication { Id = 2, MedicationName = "Ibuprofen", DoctorId = 2, ApplicationUserId = 3, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(7) }
        });
        await context.SaveChangesAsync();

        IMedicationService service = new MedicationService(context);
        
        var result = await service.GetMedicationsAsync(doctor);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetMedicationsAsync_Patient_ReturnsOnlyOwn()
    {
        var context = GetDatabaseContext();
        var patient = new ApplicationUser { Id = 1, UserName = "PatientOne", Role = UserRole.Patient, HashedPassword = "dummy" };
        
        context.Users.Add(patient);
        context.Medications.AddRange(new List<Medication>
        {
            new Medication { Id = 1, MedicationName = "Aspirin", DoctorId = 2, ApplicationUserId = 1, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(7) },
            new Medication { Id = 2, MedicationName = "Ibuprofen", DoctorId = 2, ApplicationUserId = 3, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(7) }
        });
        await context.SaveChangesAsync();

        IMedicationService service = new MedicationService(context);

        var result = await service.GetMedicationsAsync(patient);

        Assert.Single(result);
        Assert.Equal(1, result[0].ApplicationUserId);
    }

    [Fact]
    public async Task SaveMedicationAsync_Secretary_ReturnsFalse()
    {
        var context = GetDatabaseContext();
        var secretary = new ApplicationUser { Id = 4, UserName = "SecretaryOne", Role = UserRole.Secretary, HashedPassword = "dummy" };
        
        context.Users.Add(secretary);
        await context.SaveChangesAsync();

        IMedicationService service = new MedicationService(context);
        var med = new Medication 
        { 
            Id = 0, 
            MedicationName = "Tylenol", 
            Instructions = "Take as needed for pain",
            StartDate = DateTime.UtcNow, 
            EndDate = DateTime.UtcNow.AddDays(5) 
        };

        var result = await service.SaveMedicationAsync(med, secretary);

        Assert.False(result);
    }

    [Fact]
    public async Task SaveMedicationAsync_Doctor_ReturnsTrueAndSaves()
    {
        var context = GetDatabaseContext();
        var doctor = new ApplicationUser { Id = 2, UserName = "DoctorOne", Role = UserRole.Doctor, HashedPassword = "dummy" };
        
        context.Users.Add(doctor);
        await context.SaveChangesAsync();

        IMedicationService service = new MedicationService(context);
        var med = new Medication 
        { 
            Id = 0, 
            MedicationName = "Amoxicillin", 
            Instructions = "Take one pill every 8 hours",
            ApplicationUserId = 1, 
            DoctorId = 2, 
            StartDate = DateTime.UtcNow, 
            EndDate = DateTime.UtcNow.AddDays(10) 
        };

        var result = await service.SaveMedicationAsync(med, doctor);
        var savedMed = await context.Medications.FirstOrDefaultAsync(m => m.MedicationName == "Amoxicillin");

        Assert.True(result);
        Assert.NotNull(savedMed);
    }
    
    [Fact]
    public async Task DeleteMedicationAsync_ExistingId_ReturnsTrueAndDeletes()
    {
        var context = GetDatabaseContext();
        
        context.Medications.Add(new Medication 
        { 
            Id = 1, 
            MedicationName = "Aspirin",
            Instructions = "Take one tablet daily",
            ApplicationUserId = 1, 
            DoctorId = 2, 
            StartDate = DateTime.UtcNow, 
            EndDate = DateTime.UtcNow.AddDays(7) 
        });
        await context.SaveChangesAsync();

        IMedicationService service = new MedicationService(context);

        var result = await service.DeleteMedicationAsync(1);
        var deletedMed = await context.Medications.FindAsync(1);

        Assert.True(result);
        Assert.Null(deletedMed);
    }

    [Fact]
    public async Task DeleteMedicationAsync_NonExistingId_ReturnsFalse()
    {
        var context = GetDatabaseContext();
        IMedicationService service = new MedicationService(context);

        var result = await service.DeleteMedicationAsync(99);

        Assert.False(result);
    }
}