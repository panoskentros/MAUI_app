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
    
    private bool _isPatientViewVisible;
    public bool IsPatientViewVisible
    {
        get => _isPatientViewVisible;
        set { _isPatientViewVisible = value; OnPropertyChanged(); }
    }

    private bool _isSecretaryViewVisible;
    public bool IsSecretaryViewVisible
    {
        get => _isSecretaryViewVisible;
        set { _isSecretaryViewVisible = value; OnPropertyChanged(); }
    }

    private bool _isDoctorViewVisible;
    public bool IsDoctorViewVisible
    {
        get => _isDoctorViewVisible;
        set { _isDoctorViewVisible = value; OnPropertyChanged(); }
    }
    
    private string _bannerTitle = string.Empty;
    public string BannerTitle
    {
        get => _bannerTitle;
        set { _bannerTitle = value; OnPropertyChanged(); }
    }

    private string _bannerWelcomeMessage = string.Empty;
    public string BannerWelcomeMessage
    {
        get => _bannerWelcomeMessage;
        set { _bannerWelcomeMessage = value; OnPropertyChanged(); }
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
        
        IsPatientViewVisible = user.Role == UserRole.Patient;
        IsSecretaryViewVisible = user.Role == UserRole.Secretary;
        IsDoctorViewVisible = user.Role == UserRole.Doctor;

        if (IsPatientViewVisible)
        {
            BannerTitle = "Appointments";
            BannerWelcomeMessage = user.UserName;
        }
        else if (IsSecretaryViewVisible)
        {
            BannerTitle = "Clinic Appointments";
            BannerWelcomeMessage = user.UserName;
        }
        else if (IsDoctorViewVisible)
        {
            BannerTitle = "Daily Schedule";
            BannerWelcomeMessage = "Dr. " + user.UserName;
        }

        List<Appointment> appointments;

        if (IsPatientViewVisible)
        {
            appointments = await _appointmentService.GetUpcomingAppointmentsForPatientAsync(user.Id);
        }
        else if (IsSecretaryViewVisible)
        {
            appointments = await _appointmentService.GetUpcomingAppointmentsForClinicAsync(); 
        }
        else 
        {
            appointments = await _appointmentService.GetTodaysPatientsForDoctorAsync(user.Id);
        }

        var allDoctors = await _userService.GetAllDoctorsAsync();
        DailyAppointments.Clear();
        foreach (var appt in appointments)
        {
            if (IsPatientViewVisible)
            {
                var doctor = allDoctors.FirstOrDefault(d => d.Id == appt.DoctorId);
                appt.DisplayName = doctor != null ? "Dr. " + doctor.UserName : "Unknown Doctor";
            }
            else
            {
                appt.DisplayName = appt.PatientName;
            }

            DailyAppointments.Add(appt);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}