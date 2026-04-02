using MAUI_app.Controller;
using MAUI_app.Data;
using MAUI_app.Model;
using MAUI_app.Services;
using MAUI_app.View.Interfaces;

namespace MAUI_app.View;

public partial class AppointmentsPage : ContentPage, IAppointmentsView
{
    private readonly AppointmentsController _controller;
    private IRepository<Appointment> _repository;

    public AppointmentsPage(AppointmentsController controller,IRepository<Appointment> repository, IAuthService authService)
    {
        InitializeComponent();
        
        _controller = controller;
        _repository = repository;
        BindingContext = _controller; 
        if (authService.CurrentUser != null)
        {
            PageBanner.SetTitle("Appointments");
            PageBanner.SetWelcomeMessage(authService.CurrentUser.UserName);
        }
        
    }
    public async Task InitializeAsync()
    {
        
    }
    
}