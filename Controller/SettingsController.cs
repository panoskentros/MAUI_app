using MAUI_app.Model;
using MAUI_app.Data;

namespace MAUI_app.Controller;

public class SettingsController
{
    public SettingsController()
    {
    }

    public Result<UserSettings> GetCurrentUserPreferences()
    {
        var settings = new UserSettings
        {
            NotificationsEnabled = true
        };

        return Result<UserSettings>.Ok(settings, "Settings loaded");
    }

    public Result SaveUserPreferences(UserSettings settings)
    {
        if (settings == null)
        {
            return Result.Fail("Invalid settings data");
        }

        return Result.Ok("Settings saved successfully");
    }
}