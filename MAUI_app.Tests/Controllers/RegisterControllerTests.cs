using System.Threading.Tasks;
using MAUI_app.Controller;
using MAUI_app.Data;
using MAUI_app.Model;
using MAUI_app.Services.Interfaces;
using MAUI_app.View.interfaces;
using Moq;
using Xunit;

namespace MAUI_app.Tests.Controllers;

public class RegisterControllerTests
{
    [Fact]
    public async Task HandleRegisterAsync_ShowsError_WhenPasswordsDoNotMatch()
    {
        var mockView = new Mock<IRegisterView>();
        var mockUserService = new Mock<IUserService>();
        var controller = new RegisterController(mockView.Object, mockUserService.Object);

        await controller.HandleRegisterAsync("user", "email@test.com", "pass123", "pass456");

        mockView.Verify(v => v.SetConfirmPasswordError("Passwords do not match", true), Times.Once);
        mockUserService.Verify(s => s.RegisterAsync(It.IsAny<ApplicationUser>()), Times.Never);
    }

    [Fact]
    public async Task HandleRegisterAsync_CallsRegisterAndNavigatesBack_OnSuccess()
    {
        var mockView = new Mock<IRegisterView>();
        var mockUserService = new Mock<IUserService>();
        
        mockUserService
            .Setup(s => s.RegisterAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(Result.Ok(""));

        var controller = new RegisterController(mockView.Object, mockUserService.Object);

        await controller.HandleRegisterAsync("user", "user@test.com", "pass123", "pass123");

        mockView.Verify(v => v.ShowAlertAsync("Success", "Registration successful. Please log in."), Times.Once);
        mockView.Verify(v => v.ClearInputs(), Times.Once);
        mockView.Verify(v => v.NavigateBackAsync(), Times.Once);
    }
}