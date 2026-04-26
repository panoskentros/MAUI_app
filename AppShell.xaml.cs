using MAUI_app.Model;
using MAUI_app.Services;
using MAUI_app.Services.Interfaces;
using Microsoft.Maui.Controls;

namespace MAUI_app;

public partial class AppShell : Shell
{
    private readonly IUserService _userService;
    private IAsyncDisposable _asyncDisposableImplementation;

    public AppShell(IUserService userService)
    {
        InitializeComponent();
        this.Loaded += AppShell_Loaded;
        this.Unloaded += AppShell_Unloaded;
    }
    private void AppShell_Loaded(object? sender, EventArgs e)
    {
        if (Application.Current is not { } app) return;
        ThemeSwitch.IsToggled = app.RequestedTheme == AppTheme.Dark;
    }
    private void AppShell_Unloaded(object? sender, EventArgs e)
    {
        _userService.UserChanged -= UpdateMenuBasedOnRole;
        this.Loaded -= AppShell_Loaded;
        this.Unloaded -= AppShell_Unloaded;
    }
    private void UpdateMenuBasedOnRole(object? sender, EventArgs e)
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
        Application.Current.UserAppTheme = targetTheme;
    }
}