using MAUI_app.Controller;
using MAUI_app.View.interfaces;
using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
using MAUI_app.View;

namespace MAUI_app;

public partial class AppShell : Shell, IAppShellView
{
    private readonly AppShellController _controller;

    public AppShell(AppShellController controller)
    {
        InitializeComponent();
        
        _controller = controller;
        _controller.SetView(this);
        
        Routing.RegisterRoute(nameof(BookAppointmentPage), typeof(BookAppointmentPage));

        this.Loaded += AppShell_Loaded;
        this.Unloaded += AppShell_Unloaded;
    }

    private void AppShell_Loaded(object? sender, EventArgs e)
    {
        _controller.SubscribeToUserChanges();
        
        if (Application.Current is not { } app) return;
        ThemeSwitch.IsToggled = app.RequestedTheme == AppTheme.Dark;
    }

    private void AppShell_Unloaded(object? sender, EventArgs e)
    {
        _controller.UnsubscribeFromUserChanges();
        
        this.Loaded -= AppShell_Loaded;
        this.Unloaded -= AppShell_Unloaded;
    }

    public void SetScheduleVisibility(bool isVisible)
    {
        ScheduleItem.FlyoutItemIsVisible = isVisible;
    }

    public void SetAvailabilityVisibility(bool isVisible)
    {
        AvailabilityItem.FlyoutItemIsVisible = isVisible;
    }

    public void CloseFlyout()
    {
        Current.FlyoutIsPresented = false;
    }

    public async Task NavigateToLoginAsync()
    {
        await Current.GoToAsync("//LoginPage");
    }

    public void ShowError(string message)
    {
        DisplayAlert("Error", $"Logout failed: {message}", "OK");
    }

    private async void OnSignOutTapped(object sender, TappedEventArgs e)
    {
        await _controller.HandleSignOutAsync();
    }

    private void OnThemeSwitchToggled(object sender, ToggledEventArgs e)
    {
        if (Application.Current is null) return;

        var targetTheme = e.Value ? AppTheme.Dark : AppTheme.Light;

        if (Application.Current.UserAppTheme == targetTheme) return;
        Application.Current.UserAppTheme = targetTheme;
    }
}