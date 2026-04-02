using MAUI_app.Controller;
using MAUI_app.Model;
using MAUI_app.Services;

namespace MAUI_app.View;

public partial class SettingsPage : ContentPage
{
    private SettingsController _controller;

    public SettingsPage(IAuthService authService)
    {
        InitializeComponent();
        _controller = new SettingsController();

        if (authService.CurrentUser != null)
        {
            PageBanner.SetWelcomeMessage(authService.CurrentUser.UserName);
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        // MVC: Get preferences from Controller and manually update the UI
        UserSettings preferences = _controller.GetCurrentUserPreferences();
        NotificationsSwitch.IsToggled = preferences.NotificationsEnabled;
    }

    // MVC: When the user flips the switch, tell the Controller to save it!
    private void OnNotificationToggled(object sender, ToggledEventArgs e)
    {
        UserSettings updatedSettings = new UserSettings 
        { 
            NotificationsEnabled = e.Value 
        };
        
        _controller.SaveUserPreferences(updatedSettings);
    }
}