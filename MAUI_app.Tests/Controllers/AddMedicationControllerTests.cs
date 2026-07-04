using MAUI_app.Controller;
using MAUI_app.Model;
using MAUI_app.Services.Interfaces;
using MAUI_app.View.Interfaces;
using Moq;

namespace MAUI_app.Tests.Controllers;

public class AddMedicationControllerTests
{
    [Fact]
    public async Task OnViewAppearing_NewMedication_SetsCorrectButtonText()
    {
        var mockView = new Mock<IAddMedicationView>();
        var mockUserService = new Mock<IUserService>();
        var mockMedService = new Mock<IMedicationService>();

        mockUserService.Setup(s => s.GetAllPatientsAsync()).ReturnsAsync(new List<ApplicationUser>());

        var controller = new AddMedicationController(mockView.Object, mockMedService.Object, mockUserService.Object);

        await controller.OnViewAppearing();

        mockUserService.Verify(s => s.GetAllPatientsAsync(), Times.Once);
        mockView.Verify(v => v.SetSubmitButtonText("Save Prescription"), Times.Once);
    }

    [Fact]
    public async Task SaveMedication_NoPatientSelected_ShowsValidationError()
    {
        var mockView = new Mock<IAddMedicationView>();
        var mockUserService = new Mock<IUserService>();
        var mockMedService = new Mock<IMedicationService>();

        var controller = new AddMedicationController(mockView.Object, mockMedService.Object, mockUserService.Object);

        await controller.SaveMedication(null, "Aspirin", "Take 1", DateTime.UtcNow, DateTime.UtcNow.AddDays(5));

        mockView.Verify(v => v.ShowAlertAsync("Validation Error", "Please select a patient."), Times.Once);
    }

    [Fact]
    public async Task SaveMedication_UserNotDoctor_ShowsUnauthorizedError()
    {
        var mockView = new Mock<IAddMedicationView>();
        var mockUserService = new Mock<IUserService>();
        var mockMedService = new Mock<IMedicationService>();
        var patient = new ApplicationUser { Id = 1, Role = UserRole.Patient };

        mockUserService.Setup(s => s.CurrentUser).Returns(patient);

        var controller = new AddMedicationController(mockView.Object, mockMedService.Object, mockUserService.Object);

        await controller.SaveMedication(patient, "Aspirin", "Take 1", DateTime.UtcNow, DateTime.UtcNow.AddDays(5));

        mockView.Verify(v => v.ShowAlertAsync("Unauthorized", "Only doctors can prescribe medication."), Times.Once);
    }

    [Fact]
    public async Task SaveMedication_InvalidData_ShowsValidationError()
    {
        var mockView = new Mock<IAddMedicationView>();
        var mockUserService = new Mock<IUserService>();
        var mockMedService = new Mock<IMedicationService>();
        var doctor = new ApplicationUser { Id = 2, Role = UserRole.Doctor };
        var patient = new ApplicationUser { Id = 1, Role = UserRole.Patient };

        mockUserService.Setup(s => s.CurrentUser).Returns(doctor);

        var controller = new AddMedicationController(mockView.Object, mockMedService.Object, mockUserService.Object);

        await controller.SaveMedication(patient, "", "Take 1", DateTime.UtcNow, DateTime.UtcNow.AddDays(5));

        mockView.Verify(v => v.ShowAlertAsync("Validation Error", It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task SaveMedication_ValidData_SavesAndNavigatesBack()
    {
        var mockView = new Mock<IAddMedicationView>();
        var mockUserService = new Mock<IUserService>();
        var mockMedService = new Mock<IMedicationService>();
        var doctor = new ApplicationUser { Id = 2, Role = UserRole.Doctor };
        var patient = new ApplicationUser { Id = 1, Role = UserRole.Patient };

        mockUserService.Setup(s => s.CurrentUser).Returns(doctor);
        mockMedService.Setup(s => s.SaveMedicationAsync(It.IsAny<Medication>(), doctor)).ReturnsAsync(true);

        var controller = new AddMedicationController(mockView.Object, mockMedService.Object, mockUserService.Object);

        await controller.SaveMedication(patient, "Amoxicillin", "Take one pill daily", DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(10));

        mockView.Verify(v => v.ShowAlertAsync("Success", "Prescription saved successfully."), Times.Once);
        mockView.Verify(v => v.NavigateBackAsync(), Times.Once);
        mockMedService.Verify(s => s.SaveMedicationAsync(It.IsAny<Medication>(), doctor), Times.Once);
    }
}