using MAUI_app.Model;
using MAUI_app.Services;
using MAUI_app.View;

namespace MAUI_app.Controller;

public class AppointmentsController
{
    private IAuthService _authService;
    private IAppointmentsView _view;

    public AppointmentsController(IAppointmentsView view, IAuthService authService)
    {
        _view = view;
        _authService = authService;
    }
    
    public List<Appointment>? GetCurrentUserAppointments()
    {
        return  _authService.CurrentUser?.Appointments.ToList(); 
    }
    
}