using MAUI_app.Model;

namespace MAUI_app.View.interfaces;

public interface IBookAppointmentView
{
    void SetDoctors(List<ApplicationUser> doctors);
    void SetPatients(List<ApplicationUser> patients);
    void ShowPatientSelection(bool isVisible);
    void PrefillData(Appointment appt, List<ApplicationUser> doctors, List<ApplicationUser> patients);
    void SetSubmitButtonText(string text);
    Task ShowAlertAsync(string title, string message);
    Task NavigateBackAsync();
}