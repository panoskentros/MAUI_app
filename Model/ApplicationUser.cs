using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MAUI_app.Model;

public class ApplicationUser
{
    [Key]
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string HashedPassword { get; set; }
    public UserRole Role { get; set; }
    
    [InverseProperty("ApplicationUser")]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}