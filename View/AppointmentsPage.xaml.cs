using MAUI_app.Controller;
using MAUI_app.Data;
using MAUI_app.Model;
using MAUI_app.View.Interfaces;

namespace MAUI_app.View;

public partial class AppointmentsPage : ContentPage, IAppointmentsView
{
    private readonly AppointmentsController _controller;
    private IRepository<Appointment> _repository;

    public AppointmentsPage(AppointmentsController controller,IRepository<Appointment> repository)
    {
        InitializeComponent();
        
        _controller = controller;
        _repository = repository;
        BindingContext = _controller; 
    }
    public async Task InitializeAsync()
    {
        
    }
    
}