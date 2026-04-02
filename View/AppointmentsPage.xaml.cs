using MAUI_app.Controller;
using MAUI_app.Data;
using MAUI_app.Model;
using MAUI_app.Services;
using MAUI_app.View.Interfaces;

namespace MAUI_app.View;

public partial class AppointmentsPage : ContentPage, IAppointmentsView
{
    private readonly AppointmentsController _controller;

    public AppointmentsPage(AppointmentsController controller, IAuthService authService)
    {
        InitializeComponent();
        
        _controller = controller;
        BindingContext = _controller; 
        
        var user = authService.CurrentUser;
        if (user != null)
        {
            if (user.Role == UserRole.Doctor)
                PageBanner.SetTitle("Daily Schedule");
            else
                PageBanner.SetTitle("Appointments");

            PageBanner.SetWelcomeMessage(user.Role == UserRole.Doctor ? "Dr. " + user.UserName : user.UserName);
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _controller.InitializeAsync();
    }
}