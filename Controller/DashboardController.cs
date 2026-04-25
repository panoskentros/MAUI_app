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

        if (appointments != null && appointments.Any())
        {
            var nextAppt = appointments.First();
            string dateStr = nextAppt.AppointmentDate.ToString("dddd, MMM dd - h:mm tt");
            string detailsStr = "Reason: " + (string.IsNullOrWhiteSpace(nextAppt.MedicalNotes) ? "General Checkup" : nextAppt.MedicalNotes);
            
            _view.SetPatientNextAppointment(dateStr, detailsStr);
            
            bool hasMore = appointments.Count > 1;
            string buttonText = hasMore ? $"See {appointments.Count - 1} More" : "";
            _view.SetPatientMoreAppointmentsButton(hasMore, buttonText);
        }
        else
        {
            _view.SetPatientNextAppointment("No upcoming appointments", "Book a new appointment below");
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

        if (appointments != null && appointments.Any())
        {
            var nextPatient = appointments.First();
            string timeNameStr = nextPatient.AppointmentDate.ToString("h:mm tt") + " - " + nextPatient.PatientName;
            string detailsStr = "Reason: " + (string.IsNullOrWhiteSpace(nextPatient.MedicalNotes) ? "Standard Checkup" : nextPatient.MedicalNotes);

            _view.SetDoctorNextPatient(timeNameStr, detailsStr);

            bool hasMore = appointments.Count > 1;
            string buttonText = hasMore ? $"See {appointments.Count - 1} More Today" : "";
            _view.SetDoctorMorePatientsButton(hasMore, buttonText);
        }
        else
        {
            _view.SetDoctorNextPatient("No more patients today", "Enjoy your break!");
            _view.SetDoctorMorePatientsButton(false);
        }
    }

    public async Task HandleBookAppointmentClicked() => await _view.NavigateToBookAppointmentAsync();
    public async Task HandleViewAllAppointmentsClicked() => await _view.NavigateToAppointmentsAsync();
    public async Task HandleMedicalRecordsClicked() => await _view.NavigateToMedicationsAsync();
    public async Task HandleSettingsClicked() => await _view.NavigateToSettingsAsync();
}