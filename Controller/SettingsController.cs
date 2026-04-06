using MAUI_app.Model;
using MAUI_app.Services.Interfaces;

namespace MAUI_app.Controller;

public class SettingsController : BaseController
{
    public SettingsController(IUserService userService) : base(userService)
    {
    }

    public void InitializeAsync()
    {
        SetupBanner("Settings", true);
    }

    public UserSettings GetCurrentUserPreferences()
    {
        return new UserSettings
        {
            NotificationsEnabled = true
        };
    }

    public void SaveUserPreferences(UserSettings settings)
    {
    }
}