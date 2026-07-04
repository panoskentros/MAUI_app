using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using MAUI_app.Data;
using MAUI_app.Model;
using MAUI_app.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace MAUI_app.Tests.Services;

public class AppointmentServiceTests
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
    public async Task GetUpcomingAppointmentsForPatientAsync_ReturnsOnlyFutureForSpecificUser()
    {
        var context = GetDatabaseContext();
        int userId = 1;

        context.AddRange(
            new ApplicationUser { Id = userId, UserName = "Patient1", Email = "p1@test.com", HashedPassword = "hash" },
            new ApplicationUser { Id = 99, UserName = "Patient99", Email = "p99@test.com", HashedPassword = "hash" }
        );

        context.Appointments.AddRange(new List<Appointment>
        {
            new Appointment { Id = 1, ApplicationUserId = userId, AppointmentDate = DateTime.Now.AddDays(1) },
            new Appointment { Id = 2, ApplicationUserId = userId, AppointmentDate = DateTime.Now.AddDays(-1) },
            new Appointment { Id = 3, ApplicationUserId = 99, AppointmentDate = DateTime.Now.AddDays(1) }
        });
        await context.SaveChangesAsync();

        var mockValidator = new Mock<IValidator<Appointment>>();
        var service = new AppointmentService(context, mockValidator.Object);

        var results = await service.GetUpcomingAppointmentsForPatientAsync(userId);

        Assert.Single(results);
        Assert.Equal(1, results[0].Id);
    }

    [Fact]
    public async Task GetTodaysAppointmentCountAsync_ReturnsCorrectCount()
    {
        var context = GetDatabaseContext();
        
        context.AddRange(new ApplicationUser { Id = 1, UserName = "Patient1", Email = "p1@test.com", HashedPassword = "hash" });

        context.Appointments.AddRange(new List<Appointment>
        {
            new Appointment { Id = 1, ApplicationUserId = 1, AppointmentDate = DateTime.Today.AddHours(-1) },
            new Appointment { Id = 2, ApplicationUserId = 1, AppointmentDate = DateTime.Today.AddHours(1) },
            new Appointment { Id = 3, ApplicationUserId = 1, AppointmentDate = DateTime.Today.AddDays(1) }
        });
        await context.SaveChangesAsync();

        var mockValidator = new Mock<IValidator<Appointment>>();
        var service = new AppointmentService(context, mockValidator.Object);

        var count = await service.GetTodaysAppointmentCountAsync();

        Assert.Equal(1, count);
    }

    [Fact]
    public async Task GetTodaysPatientsForDoctorAsync_ReturnsCorrectAppointments()
    {
        var context = GetDatabaseContext();
        int doctorId = 5;
        var today = DateTime.Now;

        context.AddRange(new ApplicationUser { Id = 1, UserName = "Patient1", Email = "p1@test.com", HashedPassword = "hash" });

        context.Appointments.AddRange(new List<Appointment>
        {
            new Appointment { Id = 1, ApplicationUserId = 1, DoctorId = doctorId, AppointmentDate = today.AddDays(-1) },
            new Appointment { Id = 2, ApplicationUserId = 1, DoctorId = doctorId, AppointmentDate = today.AddMinutes(5) },
            new Appointment { Id = 3, ApplicationUserId = 1, DoctorId = 99, AppointmentDate = today.AddHours(12) }
        });
        await context.SaveChangesAsync();

        var mockValidator = new Mock<IValidator<Appointment>>();
        var service = new AppointmentService(context, mockValidator.Object);

        var results = await service.GetTodaysPatientsForDoctorAsync(doctorId);

        Assert.Single(results);
        Assert.Equal(2, results[0].Id);
    }

    [Fact]
    public async Task CreateAppointmentAsync_ReturnsOk_WhenValidationSucceeds()
    {
        var context = GetDatabaseContext();
        var mockValidator = new Mock<IValidator<Appointment>>();
        
        mockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var service = new AppointmentService(context, mockValidator.Object);
        
        var appointment = new Appointment 
        { 
            Id = 10, 
            ApplicationUserId = 1,
            AppointmentDate = DateTime.Today.AddDays(1), 
            DoctorId = 1
        };

        var result = await service.CreateAppointmentAsync(appointment);

        Assert.True(result.Success);
    }
    
    [Fact]
    public async Task GetPastAppointmentsForPatientAsync_ReturnsOnlyPastAppointmentsForSpecificUser()
    {
        var context = GetDatabaseContext();
        int userId = 40;

        context.AddRange(
            new ApplicationUser { Id = userId, UserName = "Patient40", Email = "p40@test.com", HashedPassword = "hash" },
            new ApplicationUser { Id = 99, UserName = "Patient99", Email = "p99@test.com", HashedPassword = "hash" }
        );

        context.Appointments.AddRange(new List<Appointment>
        {
            new Appointment { Id = 1, ApplicationUserId = userId, AppointmentDate = DateTime.Now.AddDays(-2) },
            new Appointment { Id = 2, ApplicationUserId = userId, AppointmentDate = DateTime.Now.AddDays(2) },
            new Appointment { Id = 3, ApplicationUserId = 99, AppointmentDate = DateTime.Now.AddDays(-1) }
        });
        await context.SaveChangesAsync();

        var mockValidator = new Mock<IValidator<Appointment>>();
        var service = new AppointmentService(context, mockValidator.Object);

        var results = await service.GetPastAppointmentsForPatientAsync(userId);

        Assert.Single(results);
        Assert.Equal(1, results[0].Id);
    }

    [Fact]
    public async Task GetPastAppointmentsForDoctorAsync_ReturnsOnlyPastAppointmentsForSpecificDoctor()
    {
        var context = GetDatabaseContext();
        int doctorId = 22;

        context.AddRange(new ApplicationUser { Id = 1, UserName = "Patient1", Email = "p1@test.com", HashedPassword = "hash" });

        context.Appointments.AddRange(new List<Appointment>
        {
            new Appointment { Id = 1, ApplicationUserId = 1, DoctorId = doctorId, AppointmentDate = DateTime.Now.AddDays(-3) },
            new Appointment { Id = 2, ApplicationUserId = 1, DoctorId = doctorId, AppointmentDate = DateTime.Now.AddDays(1) },
            new Appointment { Id = 3, ApplicationUserId = 1, DoctorId = 99, AppointmentDate = DateTime.Now.AddDays(-2) }
        });
        await context.SaveChangesAsync();

        var mockValidator = new Mock<IValidator<Appointment>>();
        var service = new AppointmentService(context, mockValidator.Object);

        var results = await service.GetPastAppointmentsForDoctorAsync(doctorId);

        Assert.Single(results);
        Assert.Equal(1, results[0].Id);
    }
}