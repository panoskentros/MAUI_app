namespace MAUI_app.Model;

public class CalendarDay
{
    public DateTime Date { get; set; }
    public string Day => IsEmpty ? "" : Date.Day.ToString();
    public bool HasAppointments { get; set; }
    public bool IsEmpty { get; set; }
}