using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MAUI_app.Model;

public class Appointment
{
    [Key]
    public int Id { get; set; }
    [NotMapped] public string DisplayName { get; set; } = default!;
    public DateTime AppointmentDate { get; set; }
    [MaxLength(150)]
    public string Status { get; set; } = default!;
    [MaxLength(150)]
    public string? MedicalNotes { get; set; } 
    
    public int DoctorId { get; set; }
    [ForeignKey(nameof(DoctorId))]
    public ApplicationUser Doctor { get; set; } = default!;
    
    public int ApplicationUserId { get; set; } 
    [ForeignKey(nameof(ApplicationUserId))]
    public ApplicationUser ApplicationUser { get; set; } = default!;
}