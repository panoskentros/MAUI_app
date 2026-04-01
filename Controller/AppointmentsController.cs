using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MAUI_app.Data;
using MAUI_app.Model;

namespace MAUI_app.Controller;

public class AppointmentsController
{
    private readonly IRepository<Appointment> _repository;
    
    public AppointmentsController(IRepository<Appointment> repository)
    {
        _repository = repository;

    }

    public async Task InitializeAsync()
    {
   
    }

    
}