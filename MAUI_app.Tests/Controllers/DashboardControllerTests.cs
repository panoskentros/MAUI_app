using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MAUI_app.Controller;
using MAUI_app.Model;
using MAUI_app.Services.Interfaces;
using MAUI_app.View.interfaces;
using Moq;
using Xunit;

namespace MAUI_app.Tests.Controllers;

public class DashboardControllerTests
{
    [Fact]
    public async Task InitializeDashboardAsync_ShowsPatientView_WhenUserIsPatient()
    {
        var mockView = new Mock<IDashboardView>();
        var mockUserService = new Mock<IUserService>();
        var mockAppointmentService = new Mock<IAppointmentService>();

        var patientUser = new ApplicationUser { Id = 1, Role = UserRole.Patient };
        mockUserService.Setup(s => s.CurrentUser).Returns(patientUser);

        mockAppointmentService
            .Setup(s => s.GetUpcomingAppointmentsForPatientAsync(1))
            .ReturnsAsync(new List<Appointment>());

        var controller = new DashboardController(mockView.Object, mockAppointmentService.Object, mockUserService.Object);

        await controller.InitializeDashboardAsync();

        mockView.Verify(v => v.ShowPatientView(true), Times.Once);
        mockView.Verify(v => v.ShowDoctorView(false), Times.Once);
        mockView.Verify(v => v.ShowSecretaryView(false), Times.Once);
        mockView.Verify(v => v.SetPatientNextAppointment("No upcoming appointments", "Book a new appointment below"), Times.Once);
    }

    [Fact]
    public async Task InitializeDashboardAsync_LoadsDoctorData_WhenUserIsDoctor()
    {
        var mockView = new Mock<IDashboardView>();
        var mockUserService = new Mock<IUserService>();
        var mockAppointmentService = new Mock<IAppointmentService>();

        var doctorUser = new ApplicationUser { Id = 2, Role = UserRole.Doctor };
        mockUserService.Setup(s => s.CurrentUser).Returns(doctorUser);

        var patientsList = new List<Appointment> 
        { 
            new Appointment { PatientName = "John Doe", AppointmentDate = DateTime.Today.AddHours(10), MedicalNotes = "Checkup" },
            new Appointment { PatientName = "Jane Doe", AppointmentDate = DateTime.Today.AddHours(11) }
        };

        mockAppointmentService
            .Setup(s => s.GetTodaysPatientsForDoctorAsync(2))
            .ReturnsAsync(patientsList);

        var controller = new DashboardController(mockView.Object, mockAppointmentService.Object, mockUserService.Object);

        await controller.InitializeDashboardAsync();

        mockView.Verify(v => v.ShowDoctorView(true), Times.Once);
        mockView.Verify(v => v.SetDoctorMorePatientsButton(true, "See 1 More Today"), Times.Once);
    }
}