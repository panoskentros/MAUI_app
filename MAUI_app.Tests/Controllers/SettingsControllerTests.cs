using MAUI_app.Controller;
using MAUI_app.Model;
using MAUI_app.Services.Interfaces;
using MAUI_app.View.interfaces;
using Moq;

namespace MAUI_app.Tests.Controllers;

public class SettingsControllerTests
{
    [Fact]
    public async Task HandleSaveProfileAsync_ShowsError_WhenPasswordIsTooShort()
    {
        var mockView = new Mock<ISettingsView>();
        var mockUserService = new Mock<IUserService>();
        
        var user = new ApplicationUser { Id = 1, Role = UserRole.Patient };
        mockUserService.Setup(s => s.CurrentUser).Returns(user);

        var controller = new SettingsController(mockView.Object, mockUserService.Object);

        await controller.HandleSaveProfileAsync("newName", "email@test.com", "123");

        mockView.Verify(v => v.ShowMessageAsync("Password must be at least 6 characters long.", true), Times.Once);
        mockUserService.Verify(s => s.UpdateUserAsync(It.IsAny<ApplicationUser>()), Times.Never);
    }

    [Fact]
    public async Task HandleSignOutAsync_CallsLogoutAndNavigates()
    {
        var mockView = new Mock<ISettingsView>();
        var mockUserService = new Mock<IUserService>();
        var controller = new SettingsController(mockView.Object, mockUserService.Object);

        await controller.HandleSignOutAsync();

        mockUserService.Verify(s => s.Logout(), Times.Once);
        mockView.Verify(v => v.NavigateToLoginAsync(), Times.Once);
    }
}