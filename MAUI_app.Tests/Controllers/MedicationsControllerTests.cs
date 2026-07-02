using Xunit;
using MAUI_app.Controller;
using MAUI_app.Model;
using MAUI_app.Data;
using Microsoft.EntityFrameworkCore;

namespace MAUI_app.Tests.Controllers;

public class MedicationsControllerTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        var context = new AppDbContext(options);
        
        context.Users.AddRange(
            new ApplicationUser { Id = 1, UserName = "PatientOne", Email = "p1@test.com", HashedPassword = "dummy", Role = UserRole.Patient },
            new ApplicationUser { Id = 2, UserName = "DoctorOne", Email = "d1@test.com", HashedPassword = "dummy", Role = UserRole.Doctor },
            new ApplicationUser { Id = 3, UserName = "PatientTwo", Email = "p2@test.com", HashedPassword = "dummy", Role = UserRole.Patient },
            new ApplicationUser { Id = 4, UserName = "SecretaryOne", Email = "s1@test.com", HashedPassword = "dummy", Role = UserRole.Secretary }
        );

        context.Medications.AddRange(
            new Medication { 
                Id = 1, 
                MedicationName = "Aspirin", 
                Instructions = "Take one tablet daily",
                ApplicationUserId = 1, 
                DoctorId = 2, 
                StartDate = DateTime.Now, 
                EndDate = DateTime.Now.AddDays(7) 
            },
            new Medication { 
                Id = 2, 
                MedicationName = "Ibuprofen", 
                Instructions = "Take with food",
                ApplicationUserId = 3, 
                DoctorId = 2, 
                StartDate = DateTime.Now, 
                EndDate = DateTime.Now.AddDays(7) 
            }
        );
        context.SaveChanges();
        
        return context;
    }

    [Fact]
    public async Task GetMedicationsAsync_Doctor_ReturnsAll()
    {
        var context = GetDbContext();
        var controller = new MedicationsController(context);
        var doctor = new ApplicationUser { Id = 2, Role = UserRole.Doctor };

        var result = await controller.GetMedicationsAsync(doctor);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetMedicationsAsync_Secretary_ReturnsAll()
    {
        var context = GetDbContext();
        var controller = new MedicationsController(context);
        var secretary = new ApplicationUser { Id = 4, Role = UserRole.Secretary };

        var result = await controller.GetMedicationsAsync(secretary);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetMedicationsAsync_Patient_ReturnsOnlyOwn()
    {
        var context = GetDbContext();
        var controller = new MedicationsController(context);
        var patient = new ApplicationUser { Id = 1, Role = UserRole.Patient };

        var result = await controller.GetMedicationsAsync(patient);

        Assert.Single(result);
        Assert.Equal(1, result[0].ApplicationUserId);
    }

    [Fact]
    public async Task SaveMedicationAsync_Secretary_ReturnsFalse()
    {
        var context = GetDbContext();
        var controller = new MedicationsController(context);
        var secretary = new ApplicationUser { Id = 4, Role = UserRole.Secretary };
        var med = new Medication { 
            Id = 0, 
            MedicationName = "Tylenol", 
            Instructions = "Take as needed for pain",
            StartDate = DateTime.Now, 
            EndDate = DateTime.Now.AddDays(5) 
        };

        var result = await controller.SaveMedicationAsync(med, secretary);

        Assert.False(result);
    }

    [Fact]
    public async Task SaveMedicationAsync_Doctor_ReturnsTrueAndSaves()
    {
        var context = GetDbContext();
        var controller = new MedicationsController(context);
        var doctor = new ApplicationUser { Id = 2, Role = UserRole.Doctor };
        var med = new Medication { 
            Id = 0, 
            MedicationName = "Amoxicillin", 
            Instructions = "Take one pill every 8 hours",
            ApplicationUserId = 1, 
            DoctorId = 2, 
            StartDate = DateTime.Now, 
            EndDate = DateTime.Now.AddDays(10) 
        };

        var result = await controller.SaveMedicationAsync(med, doctor);
        var savedMed = await context.Medications.FirstOrDefaultAsync(m => m.MedicationName == "Amoxicillin");

        Assert.True(result);
        Assert.NotNull(savedMed);
    }
}