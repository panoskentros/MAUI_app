using System.Threading.Tasks;
using MAUI_app.Model;
using MAUI_app.Data;
using MAUI_app.View.interfaces;

namespace MAUI_app.Controller;

public class SettingsController
{
    private readonly ISettingsView _view;

    public SettingsController(ISettingsView view)
    {
        _view = view;
    }

    public async Task InitializeDataAsync()
    {
        var settings = new UserSettings
        {
            NotificationsEnabled = true
        };

        var result = Result<UserSettings>.Ok(settings, "Settings loaded");

        if (result.Success)
        {
            _view.SetNotificationsToggleState(result.Data.NotificationsEnabled);
        }
        else
        {
            await _view.ShowErrorAsync(result.Message);
        }
    }

    public async Task HandleNotificationsChangedAsync(bool isEnabled)
    {
        var settings = new UserSettings 
        { 
            NotificationsEnabled = isEnabled 
        };

        var result = SaveUserPreferences(settings);
        
        if (!result.Success)
        {
            await _view.ShowErrorAsync(result.Message);
        }
    }

    public async Task HandleSignOutAsync()
    {
        await _view.NavigateToLoginAsync();
    }

    private Result SaveUserPreferences(UserSettings settings)
    {
        if (settings == null)
        {
            return Result.Fail("Invalid settings data");
        }

        return Result.Ok("Settings saved successfully");
    }
}