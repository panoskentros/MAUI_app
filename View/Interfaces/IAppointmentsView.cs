using MAUI_app.Model;

namespace MAUI_app.View.interfaces;

public interface IAppointmentsView
{
    void SetAppointments(List<Appointment> appointments);
    Task ShowErrorAsync(string message);
}