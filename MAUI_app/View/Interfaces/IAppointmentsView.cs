using System.Collections.ObjectModel;
using MAUI_app.Model;

namespace MAUI_app.View.interfaces;

public interface IAppointmentsView
{
    void SetAppointments(ObservableCollection<Appointment> appointments);
    Task ShowMessageAsync(string message,bool isError=false);
}