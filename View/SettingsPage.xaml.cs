using MAUI_app.Controller;
using MAUI_app.Model;
using MAUI_app.Services;

namespace MAUI_app.View;

public partial class SettingsPage : ContentPage
{
    private SettingsController _controller;
    private readonly IAuthService _authService;

    public SettingsPage(IAuthService authService)
    {
        InitializeComponent();
        _controller = new SettingsController();
        _authService = authService;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        if (_authService.CurrentUser != null)
        {
            PageBanner.SetTitle("Settings");
            PageBanner.SetWelcomeMessage(_authService.CurrentUser.UserName);
        }

        UserSettings preferences = _controller.GetCurrentUserPreferences();
        NotificationsSwitch.IsToggled = preferences.NotificationsEnabled;
    }

    private void OnNotificationToggled(object sender, ToggledEventArgs e)
    {
        UserSettings updatedSettings = new UserSettings 
        { 
            NotificationsEnabled = e.Value 
        };
        
        _controller.SaveUserPreferences(updatedSettings);
    }
}