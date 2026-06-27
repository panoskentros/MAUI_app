using System.Threading.Tasks;
using MAUI_app.Controller;
using MAUI_app.Data;
using MAUI_app.Model;
using MAUI_app.Services.Interfaces;
using MAUI_app.View.interfaces;
using Moq;
using Xunit;

namespace MAUI_app.Tests.Controllers;

public class LoginControllerTests
{
    [Fact]
    public async Task HandleLoginAsync_NavigatesToDashboard_WhenCredentialsAreValid()
    {
        var mockView = new Mock<ILoginView>();
        var mockUserService = new Mock<IUserService>();
        
        var user = new ApplicationUser { UserName = "testuser", Role = UserRole.Patient };
        mockUserService
            .Setup(s => s.LoginAsync("testuser", "password123"))
            .ReturnsAsync(Result<ApplicationUser>.Ok(user, ""));

        var controller = new LoginController(mockView.Object, mockUserService.Object);

        await controller.HandleLoginAsync("testuser", "password123");

        mockView.Verify(v => v.SetLoadingState(true), Times.Once);
        mockView.Verify(v => v.SetLoadingState(false), Times.Once);
        mockView.Verify(v => v.ClearInputs(), Times.Once);
        mockView.Verify(v => v.NavigateToDashboardAsync(), Times.Once);
    }

    [Fact]
    public async Task HandleLoginAsync_ShowsPasswordError_WhenCredentialsAreInvalid()
    {
        var mockView = new Mock<ILoginView>();
        var mockUserService = new Mock<IUserService>();
        
        mockUserService
            .Setup(s => s.LoginAsync("wronguser", "wrongpass"))
            .ReturnsAsync(Result<ApplicationUser>.Fail("Invalid credentials"));

        var controller = new LoginController(mockView.Object, mockUserService.Object);

        await controller.HandleLoginAsync("wronguser", "wrongpass");

        mockView.Verify(v => v.SetPasswordError("Invalid credentials", true), Times.Once);
        mockView.Verify(v => v.NavigateToDashboardAsync(), Times.Never);
    }

    [Fact]
    public async Task HandleLoginAsync_ShowsLocalErrors_WhenFieldsAreEmpty()
    {
        var mockView = new Mock<ILoginView>();
        var mockUserService = new Mock<IUserService>();
        var controller = new LoginController(mockView.Object, mockUserService.Object);

        await controller.HandleLoginAsync("", "");

        mockView.Verify(v => v.SetUsernameError("Please enter your username or email", true), Times.Once);
        mockView.Verify(v => v.SetPasswordError("Please enter your password", true), Times.Once);
        mockUserService.Verify(s => s.LoginAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}