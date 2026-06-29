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
        return databaseContext;
    }

    [Fact]
    public async Task GetUpcomingAppointmentsForPatientAsync_ReturnsOnlyFutureForSpecificUser()
    {
        var context = GetDatabaseContext();
        int userId = 1;

        context.Appointments.AddRange(new List<Appointment>
        {
            new Appointment { Id = 1, ApplicationUserId = userId, AppointmentDate = DateTime.Today.AddDays(-2), PatientName = "Test", Status = "Scheduled" },
            new Appointment { Id = 2, ApplicationUserId = userId, AppointmentDate = DateTime.Today.AddDays(2), PatientName = "Test", Status = "Scheduled" },
            new Appointment { Id = 3, ApplicationUserId = 99, AppointmentDate = DateTime.Today.AddDays(2), PatientName = "Test", Status = "Scheduled" }
        });
        await context.SaveChangesAsync();

        var mockValidator = new Mock<IValidator<Appointment>>();
        var service = new AppointmentService(context, mockValidator.Object);

        var results = await service.GetUpcomingAppointmentsForPatientAsync(userId);

        Assert.Single(results);
        Assert.Equal(2, results[0].Id);
    }

    [Fact]
    public async Task GetTodaysAppointmentCountAsync_ReturnsCorrectCount()
    {
        var context = GetDatabaseContext();
        
        context.Appointments.AddRange(new List<Appointment>
        {
            new Appointment { Id = 1, AppointmentDate = DateTime.Today, PatientName = "Test", Status = "Scheduled" },
            new Appointment { Id = 2, AppointmentDate = DateTime.Today, PatientName = "Test", Status = "Scheduled" },
            new Appointment { Id = 3, AppointmentDate = DateTime.Today.AddDays(1), PatientName = "Test", Status = "Scheduled" }
        });
        await context.SaveChangesAsync();

        var mockValidator = new Mock<IValidator<Appointment>>();
        var service = new AppointmentService(context, mockValidator.Object);

        var count = await service.GetTodaysAppointmentCountAsync();

        Assert.Equal(2, count);
    }

    [Fact]
    public async Task GetTodaysPatientsForDoctorAsync_ReturnsCorrectAppointments()
    {
        var context = GetDatabaseContext();
        int doctorId = 5;
        var rightNow = DateTime.Now;

        context.Appointments.AddRange(new List<Appointment>
        {
            new Appointment { Id = 1, DoctorId = doctorId, AppointmentDate = rightNow.AddHours(-1), PatientName = "Test", Status = "Scheduled" },
            new Appointment { Id = 2, DoctorId = doctorId, AppointmentDate = rightNow.AddHours(1), PatientName = "Test", Status = "Scheduled" },
            new Appointment { Id = 3, DoctorId = 99, AppointmentDate = rightNow.AddHours(1), PatientName = "Test", Status = "Scheduled" }
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
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        var service = new AppointmentService(context, mockValidator.Object);
        
        var appointment = new Appointment 
        { 
            Id = 10, 
            AppointmentDate = DateTime.Today.AddDays(1), 
            PatientName = "John Doe", 
            Status = "Scheduled",
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

        context.Appointments.AddRange(new List<Appointment>
        {
            new Appointment { Id = 1, ApplicationUserId = userId, AppointmentDate = DateTime.Now.AddDays(-2), PatientName = "Test", Status = "Completed" },
            new Appointment { Id = 2, ApplicationUserId = userId, AppointmentDate = DateTime.Now.AddDays(2), PatientName = "Test", Status = "Scheduled" },
            new Appointment { Id = 3, ApplicationUserId = 99, AppointmentDate = DateTime.Now.AddDays(-1), PatientName = "Test", Status = "Completed" }
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

        context.Appointments.AddRange(new List<Appointment>
        {
            new Appointment { Id = 1, DoctorId = doctorId, AppointmentDate = DateTime.Now.AddDays(-3), PatientName = "Test", Status = "Completed" },
            new Appointment { Id = 2, DoctorId = doctorId, AppointmentDate = DateTime.Now.AddDays(1), PatientName = "Test", Status = "Scheduled" },
            new Appointment { Id = 3, DoctorId = 99, AppointmentDate = DateTime.Now.AddDays(-2), PatientName = "Test", Status = "Completed" }
        });
        await context.SaveChangesAsync();

        var mockValidator = new Mock<IValidator<Appointment>>();
        var service = new AppointmentService(context, mockValidator.Object);

        var results = await service.GetPastAppointmentsForDoctorAsync(doctorId);

        Assert.Single(results);
        Assert.Equal(1, results[0].Id);
    }
}