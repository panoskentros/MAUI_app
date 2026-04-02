using MAUI_app.Model;
using MAUI_app.Services;
using Microsoft.Maui.Controls;

namespace MAUI_app;

public partial class AppShell : Shell
{
    private readonly IAuthService _authService;

    public AppShell(IAuthService  authService)
    {
        InitializeComponent();
        _authService = authService;
        _authService.UserChanged += (s, e) => UpdateMenuBasedOnRole();
    }
    
    private void UpdateMenuBasedOnRole()
    {
        if (_authService.CurrentUser == null) return;

        var role = _authService.CurrentUser.Role;

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
        _authService.Logout();
        await Current.GoToAsync("//LoginPage");
    }
}