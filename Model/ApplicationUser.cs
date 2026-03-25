using System.ComponentModel.DataAnnotations;

namespace MAUI_app.Model;

public class ApplicationUser
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string UserName { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string Email { get; set; }

    [Required]
    public string HashedPassword { get; set; }

    [Required]
    public UserRole Role { get; set; }
}