using MAUI_app.Model;
using MAUI_app.Services;
using MAUI_app.Services.Interfaces;
using Microsoft.Maui.Controls;

namespace MAUI_app;

public partial class AppShell : Shell
{
    private readonly IUserService _userService;

    public AppShell(IUserService  userService)
    {
        InitializeComponent();
        _userService = userService;
        _userService.UserChanged += (s, e) => UpdateMenuBasedOnRole();
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
}