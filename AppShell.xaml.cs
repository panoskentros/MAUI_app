using MAUI_app.Model;
using MAUI_app.Services;
using MAUI_app.Services.Interfaces;
using Microsoft.Maui.Controls;

namespace MAUI_app;

public partial class AppShell : Shell
{
    private readonly IUserService _userService;

    public AppShell(IUserService userService)
    {
        InitializeComponent();
        _userService = userService;
        _userService.UserChanged += (s, e) => UpdateMenuBasedOnRole();
        
        this.Loaded += AppShell_Loaded;
    }
    private void AppShell_Loaded(object? sender, EventArgs e)
    {
        if (Application.Current is not { } app) return;
        ThemeSwitch.IsToggled = app.RequestedTheme == AppTheme.Dark;
    }
    
    private void UpdateMenuBasedOnRole()
    {
        if (_userService.CurrentUser == null) return;

        var role = _userService.CurrentUser.Role;

        ScheduleItem.FlyoutItemIsVisible = false;
        AvailabilityItem.FlyoutItemIsVisible = false;

        if (role == UserRole.Secretary)
        {
            ScheduleItem.FlyoutItemIsVisible = true;
        }
        else if (role == UserRole.Doctor)
        {
            ScheduleItem.FlyoutItemIsVisible = true;
            AvailabilityItem.FlyoutItemIsVisible = true;
        }
    }
    
    private async void OnSignOutTapped(object sender, TappedEventArgs e)
    {
        Current.FlyoutIsPresented = false;
        _userService.Logout();
        await Current.GoToAsync("//LoginPage");
    }

    private void OnThemeSwitchToggled(object sender, ToggledEventArgs e)
    {
        if (Application.Current is null) return;

        var targetTheme = e.Value ? AppTheme.Dark : AppTheme.Light;

        if (Application.Current.UserAppTheme == targetTheme) return;

        Shell.Current.FlyoutIsPresented = false;

        Application.Current.Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(300), () =>
        {
            Application.Current.UserAppTheme = targetTheme;
        });
    }
}