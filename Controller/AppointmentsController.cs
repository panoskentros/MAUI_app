using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MAUI_app.Model;
using MAUI_app.Services;
using MAUI_app.Services.Interfaces;

namespace MAUI_app.Controller;

public class AppointmentsController : INotifyPropertyChanged
{
    private readonly IAppointmentService _appointmentService;
    private readonly IUserService _userService;
    
    private ObservableCollection<Appointment> _dailyAppointments = new();
    public ObservableCollection<Appointment> DailyAppointments
    {
        get => _dailyAppointments;
        set { _dailyAppointments = value; OnPropertyChanged(); }
    }

    public AppointmentsController(IAppointmentService appointmentService, IUserService userService)
    {
        _appointmentService = appointmentService;
        _userService = userService;
    }

    public async Task InitializeAsync()
    {
        var user = _userService.CurrentUser;
        if (user == null) return;

        List<Appointment> appointments;

        if (user.Role == UserRole.Patient)
        {
            appointments = await _appointmentService.GetUpcomingAppointmentsForPatientAsync(user.Id);
        }
        else if (user.Role == UserRole.Secretary)
        {
            appointments = await _appointmentService.GetUpcomingAppointmentsForClinicAsync();
        }
        else 
        {
            appointments = await _appointmentService.GetTodaysPatientsForDoctorAsync();
        }

        DailyAppointments = new ObservableCollection<Appointment>(appointments);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}