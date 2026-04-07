namespace MAUI_app.View.Interfaces;

public interface IBookAppointmentView
{
    Task ShowAlert(string title, string message,string cancelMsg);
}