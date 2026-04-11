using System;
using MAUI_app.Controller;
using MAUI_app.Model;

namespace MAUI_app.View;

public partial class SettingsPage : ContentPage
{
    private readonly SettingsController _controller;

    public SettingsPage(SettingsController controller)
    {
        InitializeComponent();
        _controller = controller;
        Title = "Settings";
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadSettings();
    }

    private void LoadSettings()
    {
        var result = _controller.GetCurrentUserPreferences();
        
        if (result.Success)
        {
            NotificationsSwitch.Toggled -= OnNotificationsChanged;
            NotificationsSwitch.IsToggled = result.Data.NotificationsEnabled;
            NotificationsSwitch.Toggled += OnNotificationsChanged;
        }
        else
        {
            DisplayAlert("Error", result.Message, "OK");
        }
    }

    private void OnNotificationsChanged(object? sender, ToggledEventArgs e)
    {
        var settings = new UserSettings 
        { 
            NotificationsEnabled = NotificationsSwitch.IsToggled 
        };

        var result = _controller.SaveUserPreferences(settings);
        
        if (!result.Success)
        {
            DisplayAlert("Error", result.Message, "OK");
        }
    }

    private async void OnSignOutClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//login"); 
    }
}