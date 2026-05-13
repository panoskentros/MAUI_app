using System.ComponentModel.DataAnnotations;

namespace MAUI_app.Model;

public class Medication
{
    [MaxLength(150)]
    public string Name { get; set; } = default!;
    [MaxLength(150)]
    public string Instructions { get; set; }  = default!;
    [MaxLength(150)]
    public string Icon { get; set; } = default!;
}