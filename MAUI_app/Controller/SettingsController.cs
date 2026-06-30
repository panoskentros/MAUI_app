using System.Threading.Tasks;
using MAUI_app.Model;
using MAUI_app.Data;
using MAUI_app.Services;
using MAUI_app.View.interfaces;
using MAUI_app.Services.Interfaces;

namespace MAUI_app.Controller;

public class SettingsController
{
    private readonly ISettingsView _view;
    private readonly IUserService _userService;

    public SettingsController(ISettingsView view, IUserService userService)
    {
        _view = view;
        _userService = userService;
    }

    public async Task InitializeDataAsync()
    {
        var user = _userService.CurrentUser;
        if (user == null)
        {
            await _view.ShowMessageAsync("User not logged in.", true);
            await _view.NavigateToLoginAsync();
            return;
        }
        _view.SetUserData(user.UserName, user.Email);
    }

    public async Task HandleSaveProfileAsync(string newUsername, string newEmail, string newPassword)
    {
        var currentUser = _userService.CurrentUser;
        if (currentUser == null)
        {
            await _view.ShowMessageAsync("Authentication error. Please log in again.", true);
            _userService.Logout();
            return;
        }

        if (!string.IsNullOrWhiteSpace(newPassword) && newPassword.Length < 6)
        {
            await _view.ShowMessageAsync("Password must be at least 6 characters long.", true);
            return;
        }

        var userToUpdate = new ApplicationUser
        {
            Id = currentUser.Id,
            UserName = newUsername.Trim(),
            Email = newEmail.Trim(),
            Role = currentUser.Role,
            HashedPassword = string.IsNullOrWhiteSpace(newPassword)
                ? currentUser.HashedPassword
                : PasswordHasher.HashPassword(newPassword)
        };

        var result = await _userService.UpdateUserAsync(userToUpdate);

        if (result.Success)
            await _view.ShowMessageAsync("Profile updated successfully!");
        else
        {
            await _view.ShowMessageAsync(result.Message, true);
            _view.SetUserData(currentUser.UserName, currentUser.Email);
        }
    }

    public async Task HandleSignOutAsync()
    {
        _userService.Logout(); 
        await _view.NavigateToLoginAsync();
    }
    
}