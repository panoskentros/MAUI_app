using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MAUI_app.Data;
using MAUI_app.Model;
using MAUI_app.Services;

namespace MAUI_app.Controller;

public class AppointmentsController : INotifyPropertyChanged
{
    private readonly IRepository<Appointment> _repository;
    private readonly IAuthService _authService;
    
    private ObservableCollection<Appointment> _dailyAppointments = new();
    public ObservableCollection<Appointment> DailyAppointments
    {
        get => _dailyAppointments;
        set { _dailyAppointments = value; OnPropertyChanged(); }
    }

    public AppointmentsController(IRepository<Appointment> repository, IAuthService authService)
    {
        _repository = repository;
        _authService = authService;
    }

    public async Task InitializeAsync()
    {
        var user = _authService.CurrentUser;
        if (user == null) return;

        IResult<IEnumerable<Appointment>> result;

        if (user.Role == UserRole.Patient)
        {
            result = await _repository.GetAllAsync(a => a.ApplicationUserId == user.Id);
            
            if (result.Success && result.Data != null)
            {
                var patientAppts = result.Data
                    .Where(a => a.AppointmentDate >= DateTime.Today)
                    .OrderBy(a => a.AppointmentDate)
                    .ToList();
                DailyAppointments = new ObservableCollection<Appointment>(patientAppts);
            }
        }
        else if (user.Role == UserRole.Secretary)
        {
            result = await _repository.GetAllAsync();
            
            if (result.Success && result.Data != null)
            {
                var clinicAppts = result.Data
                    .Where(a => a.AppointmentDate >= DateTime.Today)
                    .OrderBy(a => a.AppointmentDate)
                    .ToList();
                DailyAppointments = new ObservableCollection<Appointment>(clinicAppts);
            }
        }
        else if (user.Role == UserRole.Doctor)
        {
            result = await _repository.GetAllAsync();
            
            if (result.Success && result.Data != null)
            {
                var todaysAppts = result.Data
                    .Where(a => a.AppointmentDate.Date == DateTime.Today.Date)
                    .OrderBy(a => a.AppointmentDate)
                    .ToList();
                DailyAppointments = new ObservableCollection<Appointment>(todaysAppts);
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}