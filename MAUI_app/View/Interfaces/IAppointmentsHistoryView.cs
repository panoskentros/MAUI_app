using System.Collections.ObjectModel;
using MAUI_app.Model;

namespace MAUI_app.View.interfaces;

public interface IAppointmentsHistoryView
{
    void SetHistoryAppointments(IEnumerable<IGrouping<string, Appointment>> groupedAppointments);
}