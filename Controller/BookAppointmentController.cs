using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MAUI_app.Model;
using MAUI_app.Services.Interfaces;

namespace MAUI_app.Controller;

public class BookAppointmentController : BaseController
{
    private readonly IAppointmentService _appointmentService; 
    private readonly IUserService _userService;
    
    public BookAppointmentController(IAppointmentService appointmentService, IUserService userService) : base(userService)
    {
        _appointmentService = appointmentService;
        _userService = userService;
    }
    
    public ObservableCollection<ApplicationUser> AvailableDoctors { get; set; } = new();

    private ApplicationUser _selectedDoctor;
    public ApplicationUser SelectedDoctor
    {
        get => _selectedDoctor;
        set { _selectedDoctor = value; OnPropertyChanged(); }
    }
    
    public ObservableCollection<ApplicationUser> AvailablePatients { get; set; } = new();

    private ApplicationUser _selectedPatient;
    public ApplicationUser SelectedPatient
    {
        get => _selectedPatient;
        set { _selectedPatient = value; OnPropertyChanged(); }
    }

    private DateTime _appointmentDate = DateTime.Today.AddDays(1);
    public DateTime AppointmentDate
    {
        get => _appointmentDate;
        set { _appointmentDate = value; OnPropertyChanged(); }
    }

    private TimeSpan _appointmentTime = new TimeSpan(9, 0, 0); // Default to 9:00 AM
    public TimeSpan AppointmentTime
    {
        get => _appointmentTime;
        set { _appointmentTime = value; OnPropertyChanged(); }
    }

    private string _medicalNotes;
    public string MedicalNotes
    {
        get => _medicalNotes;
        set { _medicalNotes = value; OnPropertyChanged(); }
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
            SetupBanner("Book Appointment", false, "Schedule a Visit");
        }
        else if (IsSecretaryViewVisible)
        {
            SetupBanner("Clinic Reception", false, "Book Walk-in / Call-in");
        }
        else if (IsDoctorViewVisible)
        {
            SetupBanner("Doctor Tools", false, "Schedule Follow-up");
        }
        
        var doctors = await _userService.GetAllDoctorsAsync(); 
        AvailableDoctors.Clear();
        foreach (var doc in doctors) 
        {
            AvailableDoctors.Add(doc);
        }
        
        if (IsSecretaryViewVisible || IsDoctorViewVisible)
        {
            var patients = await _userService.GetAllPatientsAsync();
            AvailablePatients.Clear();
            foreach (var p in patients) 
            {
                AvailablePatients.Add(p);
            }
        }
    }

    public async Task<bool> SaveAppointmentAsync()
    {
        if ((IsSecretaryViewVisible || IsDoctorViewVisible) && SelectedPatient == null) return false;

        var currentUser = _userService.CurrentUser;
        
        DateTime combined = AppointmentDate.Date + AppointmentTime;
        DateTime cleanDate = new DateTime(
            combined.Year, combined.Month, combined.Day, 
            combined.Hour, combined.Minute, 0, 
            DateTimeKind.Unspecified);

        if (currentUser != null)
        {
            int correctPatientId = IsPatientViewVisible ? currentUser.Id : SelectedPatient.Id;
            string correctPatientName = IsPatientViewVisible ? currentUser.UserName : SelectedPatient.UserName;

            var newAppointment = new Appointment
            {
                ApplicationUserId = correctPatientId, 
                DoctorId = SelectedDoctor.Id,       
                PatientName = correctPatientName,
                AppointmentDate = cleanDate,
                MedicalNotes = this.MedicalNotes,
                Status = "Scheduled"
            };
        
            await _appointmentService.CreateAppointmentAsync(newAppointment);
        }

        return true;
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}