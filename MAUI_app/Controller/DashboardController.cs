using System.Linq;
using System.Threading.Tasks;
using MAUI_app.Model;
using MAUI_app.Services.Interfaces;
using MAUI_app.View.interfaces;
using MAUI_app.Data;

namespace MAUI_app.Controller;

public class DashboardController
{
    private readonly IDashboardView _view;
    private readonly IAppointmentService _appointmentService;
    private readonly IUserService _userService;
    private Appointment _nextPatientAppointment;
    private Appointment _nextDoctorAppointment;

    public DashboardController(
        IDashboardView view, 
        IAppointmentService appointmentService, 
        IUserService userService)
    {
        _view = view;
        _appointmentService = appointmentService;
        _userService = userService;
    }

    public async Task InitializeDashboardAsync()
    {
        var user = _userService.CurrentUser;
        if (user == null) return;

        // Ορίζουμε ποιο View φαίνεται
        _view.ShowPatientView(user.Role == UserRole.Patient);
        _view.ShowSecretaryView(user.Role == UserRole.Secretary);
        _view.ShowDoctorView(user.Role == UserRole.Doctor);

        switch (user.Role)
        {
            case UserRole.Patient:
                await LoadPatientDataAsync(user.Id);
                break;
                
            case UserRole.Secretary:
                await LoadSecretaryDataAsync();
                break;
                
            case UserRole.Doctor:
                await LoadDoctorDataAsync(user.Id);
                break;
        }
    }

    private async Task LoadPatientDataAsync(int userId)
    {
        var appointments = await _appointmentService.GetUpcomingAppointmentsForPatientAsync(userId);

        if (appointments.Any())
        {
            var nextAppt = appointments.First();
            _nextPatientAppointment = nextAppt;
            string dateStr = nextAppt.AppointmentDate.ToString("dddd, MMM dd - h:mm tt");
            string detailsStr = "Reason: " + (string.IsNullOrWhiteSpace(nextAppt.MedicalNotes) ? "General Checkup" : nextAppt.MedicalNotes);
            
            _view.SetPatientNextAppointment(dateStr, detailsStr);
            _view.SetPatientRescheduleButton(true);
            
            bool hasMore = appointments.Count > 1;
            string buttonText = hasMore ? $"See {appointments.Count - 1} More" : "";
            _view.SetPatientMoreAppointmentsButton(hasMore, buttonText);
        }
        else
        {   
            _nextPatientAppointment = null;
            _view.SetPatientNextAppointment("No upcoming appointments", "Book a new appointment below");
            _view.SetPatientRescheduleButton(false);
            _view.SetPatientMoreAppointmentsButton(false);
        }
    }

    private async Task LoadSecretaryDataAsync()
    {
        var count = await _appointmentService.GetTodaysAppointmentCountAsync();
        _view.SetSecretaryAppointmentsToday(count.ToString());
    }

    private async Task LoadDoctorDataAsync(int userId)
    {
        var appointments = await _appointmentService.GetTodaysPatientsForDoctorAsync(userId);

        if (appointments.Any())
        {
            var nextAppointment = appointments.First();
            _nextDoctorAppointment = nextAppointment;
            
            string timeNameStr = nextAppointment.AppointmentDate.ToString("dddd, MMM dd - h:mm tt") + " - " + nextAppointment.ApplicationUser?.UserName;
            string detailsStr = "Reason: " + (string.IsNullOrWhiteSpace(nextAppointment.MedicalNotes) ? "Standard Checkup" : nextAppointment.MedicalNotes);

            _view.SetDoctorNextPatient(timeNameStr, detailsStr);

            bool hasMore = appointments.Count > 1;
            string buttonText = hasMore ? $"See {appointments.Count - 1} More" : "";
            _view.SetDoctorMorePatientsButton(hasMore, buttonText);
        }
        else
        {
            _nextDoctorAppointment = null;
            _view.SetDoctorNextPatient("No more patients today", "Enjoy your break!");
            _view.SetDoctorMorePatientsButton(false);
        }
    }

    public async Task HandleBookAppointmentClicked() => await _view.NavigateToBookAppointmentAsync();
    public async Task HandleViewAllAppointmentsClicked() => await _view.NavigateToAppointmentsAsync();
    public async Task HandleMedicalRecordsClicked() => await _view.NavigateToMedicationsAsync();
    public async Task HandleRescheduleClicked()
    {
        if (_nextPatientAppointment != null)
        {
            await _view.NavigateToBookAppointmentAsync(_nextPatientAppointment);
        }
    }
    public async Task HandleWriteDiagnosisClicked()
    {
        await _view.NavigateToAddMedicationAsync();
    }
}