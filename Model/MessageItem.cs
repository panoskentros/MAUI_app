namespace MAUI_app.Model;

public class MessageItem
{
    public string Sender { get; set; }
    public string Time { get; set; }
    public string Subject { get; set; }
    public string PreviewText { get; set; }
    public string BorderColor { get; set; } // We use this to color unread messages blue!
}