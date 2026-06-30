using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MAUI_app.Controller;
using MAUI_app.Model;
using MAUI_app.Services.Interfaces;
using MAUI_app.View.interfaces;
using Moq;
using Xunit;

namespace MAUI_app.Tests.Controllers;

public class AppointmentsHistoryControllerTests
{
    private readonly Mock<IAppointmentsHistoryView> _mockView;
    private readonly Mock<IAppointmentService> _mockAppointmentService;
    private readonly Mock<IUserService> _mockUserService;

    public AppointmentsHistoryControllerTests()
    {
        _mockView = new Mock<IAppointmentsHistoryView>();
        _mockAppointmentService = new Mock<IAppointmentService>();
        _mockUserService = new Mock<IUserService>();
    }

    [Fact]
    public async Task LoadHistoryAsync_WhenUserIsPatient_GroupsAppointmentsCorrectly()
    {
        var currentPatient = new ApplicationUser { Id = 40, Role = UserRole.Patient, UserName = "PatientTest" };
        _mockUserService.Setup(s => s.CurrentUser).Returns(currentPatient);

        var pastDate = DateTime.Now.AddDays(-5);
        var pastAppointments = new List<Appointment>
        {
            new Appointment { Id = 1, ApplicationUserId = 40, DoctorId = 22, AppointmentDate = pastDate}
        };
        _mockAppointmentService.Setup(s => s.GetPastAppointmentsForPatientAsync(40)).ReturnsAsync(pastAppointments);

        var doctorsList = new List<ApplicationUser>
        {
            new ApplicationUser { Id = 22, Role = UserRole.Doctor, UserName = "Smith" }
        };
        _mockUserService.Setup(s => s.GetAllDoctorsAsync()).ReturnsAsync(doctorsList);

        var controller = new AppointmentsHistoryController(_mockView.Object, _mockAppointmentService.Object, _mockUserService.Object);

        await controller.LoadHistoryAsync();

        _mockView.Verify(v => v.SetHistoryAppointments(It.Is<IEnumerable<IGrouping<string, Appointment>>>(
            g => g.Count() == 1 && g.First().Key == pastDate.Date.ToString("MMMM dd, yyyy") && g.First().First().DisplayName == "Dr. Smith"
        )), Times.Once);
    }

    [Fact]
    public async Task LoadHistoryAsync_WhenUserIsDoctor_GroupsAppointmentsCorrectly()
    {
        var currentDoctor = new ApplicationUser { Id = 22, Role = UserRole.Doctor, UserName = "DoctorTest" };
        _mockUserService.Setup(s => s.CurrentUser).Returns(currentDoctor);

        var pastDate = DateTime.Now.AddDays(-2);
        var pastAppointments = new List<Appointment>
        {
            new Appointment { Id = 2, ApplicationUserId = 40, DoctorId = 22, AppointmentDate = pastDate }
        };
        _mockAppointmentService.Setup(s => s.GetPastAppointmentsForDoctorAsync(22)).ReturnsAsync(pastAppointments);

        var controller = new AppointmentsHistoryController(_mockView.Object, _mockAppointmentService.Object, _mockUserService.Object);

        await controller.LoadHistoryAsync();

        _mockView.Verify(v => v.SetHistoryAppointments(It.Is<IEnumerable<IGrouping<string, Appointment>>>(
            g => g.Count() == 1 && g.First().Key == pastDate.Date.ToString("MMMM dd, yyyy") && g.First().First().DisplayName == "Panos Kentros"
        )), Times.Once);
    }
}