using System.ComponentModel.DataAnnotations.Schema;
namespace MAUI_app.Model;

public class Appointment
{
    public int Id { get; set; }
    public int DoctorId { get; set; }
    public string PatientName { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string Status { get; set; } 
    public string MedicalNotes { get; set; } 
    
    [Column("ApplicationUserId")]
    public int ApplicationUserId { get; set; }
    
    public ApplicationUser ApplicationUser { get; set; }
}