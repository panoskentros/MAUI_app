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
    }
    
    private async void OnSignOutTapped(object sender, TappedEventArgs e)
    {
        Current.FlyoutIsPresented = false;
        _authService.Logout();
        await Current.GoToAsync("//LoginPage");
    }
}