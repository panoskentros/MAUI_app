using System.ComponentModel;
using System.Runtime.CompilerServices;
using MAUI_app.Data;
using MAUI_app.Model;
using MAUI_app.Services;
using MAUI_app.Services.Interfaces;

namespace MAUI_app.Controller;

public class DashboardController : BaseController
{
    private string _patientNextAppointmentDate = "No upcoming appointments";
    public string PatientNextAppointmentDate
    {
        get => _patientNextAppointmentDate;
        set { _patientNextAppointmentDate = value; OnPropertyChanged(); }
    }

    private string _patientNextAppointmentDetails = "Book a new appointment below";
    public string PatientNextAppointmentDetails
    {
        get => _patientNextAppointmentDetails;
        set { _patientNextAppointmentDetails = value; OnPropertyChanged(); }
    }

    private string _appointmentsTodayCount = "0";
    public string AppointmentsTodayCount
    {
        get => _appointmentsTodayCount;
        set { _appointmentsTodayCount = value; OnPropertyChanged(); }
    }

    private string _doctorNextPatientTime = "No more patients today";
    public string DoctorNextPatientTime
    {
        get => _doctorNextPatientTime;
        set { _doctorNextPatientTime = value; OnPropertyChanged(); }
    }

    private string _doctorNextPatientDetails = "Enjoy your break!";
    public string DoctorNextPatientDetails
    {
        get => _doctorNextPatientDetails;
        set { _doctorNextPatientDetails = value; OnPropertyChanged(); }
    }
    
    private bool _hasMoreAppointments;
    public bool HasMoreAppointments
    {
        get => _hasMoreAppointments;
        set { _hasMoreAppointments = value; OnPropertyChanged(); }
    }

    private string _moreAppointmentsText;
    public string MoreAppointmentsText
    {
        get => _moreAppointmentsText;
        set { _moreAppointmentsText = value; OnPropertyChanged(); }
    }
    
    private bool _doctorHasMorePatients;
    public bool DoctorHasMorePatients
    {
        get => _doctorHasMorePatients;
        set { _doctorHasMorePatients = value; OnPropertyChanged(); }
    }

    private string _doctorMorePatientsText;
    public string DoctorMorePatientsText
    {
        get => _doctorMorePatientsText;
        set { _doctorMorePatientsText = value; OnPropertyChanged(); }
    }

    private readonly IAppointmentService _appointmentService;
    private readonly IUserService _userService;

    public DashboardController(IAppointmentService appointmentService, IUserService userService) : base(userService)
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
            SetupBanner("Patient Dashboard", true);

            var upcomingAppts = await _appointmentService.GetUpcomingAppointmentsForPatientAsync(user.Id);

            if (upcomingAppts.Any())
            {
                var nextAppt = upcomingAppts.First();
                PatientNextAppointmentDate = nextAppt.AppointmentDate.ToString("dddd, MMM dd - h:mm tt");
                PatientNextAppointmentDetails = "Reason: " + (string.IsNullOrWhiteSpace(nextAppt.MedicalNotes) ? "General Checkup" : nextAppt.MedicalNotes);
                
                HasMoreAppointments = upcomingAppts.Count > 1;
                if (HasMoreAppointments)
                {
                    MoreAppointmentsText = $"See {upcomingAppts.Count - 1} More";
                }
            }
            else
            {
                PatientNextAppointmentDate = "No upcoming appointments";
                PatientNextAppointmentDetails = "Book a new appointment below";
                HasMoreAppointments = false;
            }
        }
        else if (IsSecretaryViewVisible)
        {
            SetupBanner("Clinic Control Center", true);

            int count = await _appointmentService.GetTodaysAppointmentCountAsync();
            AppointmentsTodayCount = count.ToString();
        }
        else if (IsDoctorViewVisible)
        {
            SetupBanner("Doctor Dashboard", true);

            var todaysPatients = await _appointmentService.GetTodaysPatientsForDoctorAsync(user.Id);

            if (todaysPatients.Any())
            {
                var nextPatient = todaysPatients.First();
                DoctorNextPatientTime = nextPatient.AppointmentDate.ToString("h:mm tt") + " - " + nextPatient.PatientName;
                DoctorNextPatientDetails = "Reason: " + (string.IsNullOrWhiteSpace(nextPatient.MedicalNotes) ? "Standard Checkup" : nextPatient.MedicalNotes);

                DoctorHasMorePatients = todaysPatients.Count > 1;
                if (DoctorHasMorePatients)
                {
                    DoctorMorePatientsText = $"See {todaysPatients.Count - 1} More Today";
                }
            }
            else
            {
                DoctorNextPatientTime = "No more patients today";
                DoctorNextPatientDetails = "Enjoy your break!";
                DoctorHasMorePatients = false;
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}