using MAUI_app.Controller;
using MAUI_app.Services;

namespace MAUI_app.View;

public partial class AppointmentsPage : ContentPage
{
    private AppointmentsController _controller;
    private readonly IAuthService _authService;

    public AppointmentsPage(IAuthService authService)
    {
        InitializeComponent();
        _authService = authService;
        
    }

   

    
}