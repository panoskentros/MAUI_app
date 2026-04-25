using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MAUI_app.Controller;
using MAUI_app.Model;
using MAUI_app.View.interfaces;

namespace MAUI_app.View;

public partial class SettingsPage : ContentPage, ISettingsView
{
    private readonly SettingsController _controller;

    public SettingsPage()
    {
        InitializeComponent();
        _controller = new SettingsController(this);
        Title = "Settings";
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _controller.InitializeDataAsync();
    }
    private async void OnNotificationsChanged(object? sender, ToggledEventArgs e)
    {
        await _controller.HandleNotificationsChangedAsync(e.Value);
    }
    private async void OnSignOutClicked(object? sender, EventArgs e)
    {
        await _controller.HandleSignOutAsync();
    }

    public void SetNotificationsToggleState(bool isEnabled)
    {
        NotificationsSwitch.Toggled -= OnNotificationsChanged;
        NotificationsSwitch.IsToggled = isEnabled;
        NotificationsSwitch.Toggled += OnNotificationsChanged;
    }

    public Task ShowErrorAsync(string message)
    {
        return DisplayAlert("Error", message, "OK");
    }

    public Task NavigateToLoginAsync()
    {
        return Shell.Current.GoToAsync("//login"); 
    }
}