namespace MAUI_app.Model;

public class Appointment
{
    public int Id { get; set; }
    public string PatientName { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string Status { get; set; } 
    public string MedicalNotes { get; set; } 
}