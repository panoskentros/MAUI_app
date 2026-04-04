using MAUI_app.Controller;
using MAUI_app.Data;
using MAUI_app.Model;
using MAUI_app.Services;
using MAUI_app.View.Interfaces;

namespace MAUI_app.View;

public partial class AppointmentsPage : ContentPage, IAppointmentsView
{
    private readonly AppointmentsController _controller;
    private readonly IUserService _userService;

    public AppointmentsPage(AppointmentsController controller, IUserService userService)
    {
        InitializeComponent();
        
        _controller = controller;
        _userService = userService;
        BindingContext = _controller; 
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        var user = _userService.CurrentUser;
        if (user != null)
        {
            if (user.Role == UserRole.Doctor)
                PageBanner.SetTitle("Daily Schedule"); 
            else
                PageBanner.SetTitle("Appointments");

            PageBanner.SetWelcomeMessage(user.Role == UserRole.Doctor ? "Dr. " + user.UserName : user.UserName);
        }

        await _controller.InitializeAsync();
    }
}