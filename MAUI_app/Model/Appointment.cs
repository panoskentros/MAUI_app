using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MAUI_app.Model;

public class Appointment
{
    public int Id { get; set; }
    public int DoctorId { get; set; }
    [MaxLength(150)]
    public string PatientName { get; set; } = default!;
    [NotMapped] public string DisplayName { get; set; } = default!;
    public DateTime AppointmentDate { get; set; }
    [MaxLength(150)]
    public string Status { get; set; } = default!;
    [MaxLength(150)]
    public string? MedicalNotes { get; set; } 
    [Column("ApplicationUserId")]
    public int ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; } = default!;
}