using System.ComponentModel.DataAnnotations;

namespace MAUI_app.Model;

public class MessageItem
{
    [MaxLength(150)]
    public string Sender { get; set; } = default!;
    [MaxLength(150)]
    public string Time { get; set; } = default!;
    [MaxLength(150)]
    public string Subject { get; set; } = default!;
    [MaxLength(150)]
    public string PreviewText { get; set; }  = default!;
    [MaxLength(150)]
    public string BorderColor { get; set; } = default!;
}