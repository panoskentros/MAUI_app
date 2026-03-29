using MAUI_app.Model;

namespace MAUI_app.Controller;

public class SettingsController
{
    public UserSettings GetCurrentUserPreferences()
    {
        // Mock database call to get the user's saved preferences
        return new UserSettings
        {
            NotificationsEnabled = true
        };
    }

    public void SaveUserPreferences(UserSettings settings)
    {
        // Here you would save the changes back to your database!
    }
}