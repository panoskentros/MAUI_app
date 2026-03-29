using MAUI_app.Model;
using MAUI_app.Services;

namespace MAUI_app.View;

public partial class DashboardPage : ContentPage
{
    private ApplicationUser? _currentUser; 

    public DashboardPage(IAuthService authService)
    {
        InitializeComponent();    
        _currentUser = authService.CurrentUser;
        
        if (_currentUser != null)
        {
            // Send the name to our new component!
            PageBanner.SetWelcomeMessage(_currentUser.UserName);
        }
    }
}