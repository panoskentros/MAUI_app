using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MAUI_app.Model;

public class ApplicationUser
{
    [Key]
    public int Id { get; set; }
    
    [MaxLength(150)]
    public string UserName { get; set; } = default!;
    
    [MaxLength(150)]
    public string Email { get; set; } = default!;
    
    [MaxLength(150)]
    public string HashedPassword { get; set; } = default!;
    
    public UserRole Role { get; set; }
    
    [InverseProperty("ApplicationUser")]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    
    [InverseProperty("Doctor")]
    public virtual ICollection<Appointment> DoctorAppointments { get; set; } = new List<Appointment>();
    
    [InverseProperty("Patient")]
    public virtual ICollection<Medication> Medications { get; set; } = new List<Medication>();
    
    [InverseProperty("Doctor")]
    public virtual ICollection<Medication> PrescribedMedications { get; set; } = new List<Medication>();
}