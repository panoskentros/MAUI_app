using MAUI_app.Controller;
using MAUI_app.Data;
using MAUI_app.Model;
using MAUI_app.Services.Interfaces;
using MAUI_app.View.interfaces;
using Moq;

namespace MAUI_app.Tests.Controllers;

public class BookAppointmentControllerTests
{
    [Fact]
    public async Task OnViewAppearing_AsDoctor_LoadsPatientsAndDoctors()
    {
        var mockView = new Mock<IBookAppointmentView>();
        var mockApptService = new Mock<IAppointmentService>();
        var mockUserService = new Mock<IUserService>();
        var doctor = new ApplicationUser { Id = 2, Role = UserRole.Doctor };

        mockUserService.Setup(s => s.CurrentUser).Returns(doctor);
        mockUserService.Setup(s => s.GetAllDoctorsAsync()).ReturnsAsync(new List<ApplicationUser>());
        mockUserService.Setup(s => s.GetAllPatientsAsync()).ReturnsAsync(new List<ApplicationUser>());

        var controller = new BookAppointmentController(mockView.Object, mockApptService.Object, mockUserService.Object);

        await controller.OnViewAppearing();

        mockView.Verify(v => v.ShowPatientSelection(true), Times.Once);
        mockUserService.Verify(s => s.GetAllDoctorsAsync(), Times.Once);
        mockUserService.Verify(s => s.GetAllPatientsAsync(), Times.Once);
        mockView.Verify(v => v.SetSubmitButtonText("Confirm Appointment"), Times.Once);
    }

    [Fact]
    public async Task SaveAppointment_NoUserAuthenticated_ShowsError()
    {
        var mockView = new Mock<IBookAppointmentView>();
        var mockApptService = new Mock<IAppointmentService>();
        var mockUserService = new Mock<IUserService>();

        mockUserService.Setup(s => s.CurrentUser).Returns((ApplicationUser)null);

        var controller = new BookAppointmentController(mockView.Object, mockApptService.Object, mockUserService.Object);

        await controller.SaveAppointment(new ApplicationUser(), new ApplicationUser(), DateTime.Today, new TimeSpan(10, 0, 0), "Test");

        mockView.Verify(v => v.ShowAlertAsync("Error", "User not authenticated."), Times.Once);
    }

    [Fact]
    public async Task SaveAppointment_MissingDoctor_ShowsError()
    {
        var mockView = new Mock<IBookAppointmentView>();
        var mockApptService = new Mock<IAppointmentService>();
        var mockUserService = new Mock<IUserService>();
        var patient = new ApplicationUser { Id = 1, Role = UserRole.Patient };

        mockUserService.Setup(s => s.CurrentUser).Returns(patient);

        var controller = new BookAppointmentController(mockView.Object, mockApptService.Object, mockUserService.Object);

        await controller.SaveAppointment(null, null, DateTime.Today, new TimeSpan(10, 0, 0), "Test");

        mockView.Verify(v => v.ShowAlertAsync("Error", "Please select a doctor."), Times.Once);
    }

    [Fact]
    public async Task SaveAppointment_ValidNewAppointment_CreatesAndNavigatesBack()
    {
        var mockView = new Mock<IBookAppointmentView>();
        var mockApptService = new Mock<IAppointmentService>();
        var mockUserService = new Mock<IUserService>();
        var patient = new ApplicationUser { Id = 1, Role = UserRole.Patient };
        var doctor = new ApplicationUser { Id = 2, Role = UserRole.Doctor };
        
        mockUserService.Setup(s => s.CurrentUser).Returns(patient);
        mockApptService
            .Setup(s => s.CreateAppointmentAsync(It.IsAny<Appointment>()))
            .ReturnsAsync(Result<Appointment>.Ok(new Appointment(), "Success"));

        var controller = new BookAppointmentController(mockView.Object, mockApptService.Object, mockUserService.Object);

        await controller.SaveAppointment(doctor, null, DateTime.Today, new TimeSpan(10, 0, 0), "Checkup");

        mockApptService.Verify(s => s.CreateAppointmentAsync(It.IsAny<Appointment>()), Times.Once);
        mockView.Verify(v => v.ShowAlertAsync("Success", "Success"), Times.Once);
        mockView.Verify(v => v.NavigateBackAsync(), Times.Once);
    }

    [Fact]
    public async Task SaveAppointment_EditingExisting_UpdatesAndNavigatesBack()
    {
        var mockView = new Mock<IBookAppointmentView>();
        var mockApptService = new Mock<IAppointmentService>();
        var mockUserService = new Mock<IUserService>();
        var doctor = new ApplicationUser { Id = 2, Role = UserRole.Doctor };
        var patient = new ApplicationUser { Id = 1, Role = UserRole.Patient };
        
        mockUserService.Setup(s => s.CurrentUser).Returns(doctor);
        mockApptService
            .Setup(s => s.UpdateAppointmentAsync(It.IsAny<Appointment>()))
            .ReturnsAsync(Result<Appointment>.Ok(new Appointment(), "Updated"));

        var controller = new BookAppointmentController(mockView.Object, mockApptService.Object, mockUserService.Object);
        controller.SetAppointmentToEdit(new Appointment { Id = 5 });

        await controller.SaveAppointment(doctor, patient, DateTime.Today, new TimeSpan(10, 0, 0), "Checkup");

        mockApptService.Verify(s => s.UpdateAppointmentAsync(It.IsAny<Appointment>()), Times.Once);
        mockView.Verify(v => v.ShowAlertAsync("Success", "Updated"), Times.Once);
        mockView.Verify(v => v.NavigateBackAsync(), Times.Once);
    }
}