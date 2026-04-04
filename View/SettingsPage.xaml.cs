using MAUI_app.Controller;
using MAUI_app.Model;
using MAUI_app.Services;

namespace MAUI_app.View;

public partial class SettingsPage : ContentPage
{
    private SettingsController _controller;
    private readonly IUserService _userService;

    public SettingsPage(IUserService userService)
    {
        InitializeComponent();
        _controller = new SettingsController();
        _userService = userService;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        if (_userService.CurrentUser != null)
        {
            PageBanner.SetTitle("Settings");
            PageBanner.SetWelcomeMessage(_userService.CurrentUser.UserName);
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