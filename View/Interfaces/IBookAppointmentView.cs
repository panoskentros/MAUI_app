using MAUI_app.Model;

namespace MAUI_app.View.interfaces;

public interface IBookAppointmentView
{
    void SetDoctors(List<ApplicationUser> doctors);
    void SetPatients(List<ApplicationUser> patients);
    void ShowPatientSelection(bool isVisible);
    Task ShowAlertAsync(string title, string message);
    Task NavigateBackAsync();
}