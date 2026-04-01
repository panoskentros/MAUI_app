namespace MAUI_app.Controller;

public class DashboardController
{
    public string GetCurrentUserRole()
    {
        // 🛑 FOR TESTING: Change to "Doctor" or "Secretary" to test the UI!
        return "Patient"; 
    }

    // Patient Summary Data
    public string GetNextAppointmentPatient()
    {
        return "Tomorrow at 10:00 AM - Dr. Sarah Jenkins";
    }

    public int GetUnreadMessagesCount()
    {
        return 2;
    }

    // Doctor Summary Data
    public string GetTodayStatsDoctor()
    {
        return "You have 6 appointments today.";
    }

    public string GetNextPatient()
    {
        return "10:00 AM - John Doe (Checkup)";
    }
}