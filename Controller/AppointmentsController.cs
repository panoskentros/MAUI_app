using MAUI_app.Model;

namespace MAUI_app.Controller;

public class AppointmentsController
{
    // 1. Figure out who is logged in
    public string GetCurrentUserRole()
    {
        // 🛑 FOR TESTING: Change this to "Doctor" to test the other UI
        return "Patient"; 
    }

    // 2. Fetch Patient Data
    public List<Appointment> GetPatientAppointments()
    {
        return new List<Appointment>
        {
            new Appointment { DoctorName = "Dr. Sarah Jenkins", AppointmentDate = DateTime.Now.AddDays(1), Status = "Upcoming" },
            new Appointment { DoctorName = "Dr. Marcus Welby", AppointmentDate = DateTime.Now.AddDays(14), Status = "Upcoming" }
        };
    }

    // 3. Fetch Doctor Data
    public List<Appointment> GetDoctorSchedule()
    {
        return new List<Appointment>
        {
            new Appointment { PatientName = "John Doe", AppointmentDate = DateTime.Now.AddHours(2), Status = "Upcoming" },
            new Appointment { PatientName = "Jane Smith", AppointmentDate = DateTime.Now.AddHours(3), Status = "Upcoming" }
        };
    }
}