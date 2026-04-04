using MAUI_app.Controller;
using MAUI_app.Data;
using MAUI_app.Model;
using MAUI_app.Services;
using MAUI_app.Services.Interfaces;
using MAUI_app.View.Interfaces;

namespace MAUI_app.View;

public partial class MyAppointmentsPage : ContentPage, IAppointmentsView
{
    private readonly AppointmentsController _controller;
    private readonly IUserService _userService;

    public MyAppointmentsPage(AppointmentsController controller, IUserService userService)
    {
        InitializeComponent();
        
        _controller = controller;
        _userService = userService;
        BindingContext = _controller; 
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _controller.InitializeAsync();
    }
}