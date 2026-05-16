using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MAUI_app.Controller;
using MAUI_app.Data;
using MAUI_app.Model;
using MAUI_app.Services.Interfaces;
using MAUI_app.View.interfaces;
using Moq;
using Xunit;

namespace MAUI_app.Tests.Controllers;

public class AppointmentsControllerTests
{
    [Fact]
    public async Task InitializeDataAsync_ShowsError_WhenUserNotLoggedIn()
    {
        var mockView = new Mock<IAppointmentsView>();
        var mockUserService = new Mock<IUserService>();
        var mockApptService = new Mock<IAppointmentService>();

        mockUserService.Setup(s => s.CurrentUser).Returns((ApplicationUser?)null);

        var controller = new AppointmentsController(mockView.Object, mockApptService.Object, mockUserService.Object);

        await controller.InitializeDataAsync();

        mockView.Verify(v => v.ShowMessageAsync("User not logged in", true), Times.Once);
        mockApptService.Verify(s => s.GetUpcomingAppointmentsForPatientAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task CancelAppointmentAsync_RemovesFromCollection_WhenSuccessful()
    {
        var mockView = new Mock<IAppointmentsView>();
        var mockUserService = new Mock<IUserService>();
        var mockApptService = new Mock<IAppointmentService>();

        var appointmentToCancel = new Appointment { Id = 99 };
        
        mockApptService
            .Setup(s => s.CancelAppointmentAsync(99))
            .ReturnsAsync(Result.Ok("Cancelled successfully"));

        var controller = new AppointmentsController(mockView.Object, mockApptService.Object, mockUserService.Object);
        controller.appointments = new ObservableCollection<Appointment> { appointmentToCancel };

        await controller.CancelAppointmentAsync(appointmentToCancel);

        mockView.Verify(v => v.ShowMessageAsync("Cancelled successfully", false), Times.Once);
        Assert.Empty(controller.appointments);
    }
}