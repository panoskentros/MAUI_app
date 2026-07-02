
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MAUI_app.Model;

public class Medication
{
    [Key] public int Id { get; set; }
    public string MedicationName { get; set; } = default!;
    public string Instructions { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int DoctorId { get; set; }
    [ForeignKey(nameof(DoctorId))]
    public ApplicationUser Doctor { get; set; } = default!;
    
    public int ApplicationUserId { get; set; } 
    [ForeignKey(nameof(ApplicationUserId))]
    public ApplicationUser Patient { get; set; } = default!;
    [NotMapped]
    public string DisplayName { get; set; }
}