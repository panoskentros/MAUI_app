using MAUI_app.Model;

namespace MAUI_app.Controller;

public class MessagesController
{
    public List<MessageItem> GetInboxMessages()
    {
        // Mock database call
        return new List<MessageItem>
        {
            new MessageItem { Sender = "Dr. Sarah Jenkins", Time = "10:30 AM", Subject = "Lab Results are ready", PreviewText = "Your recent blood work results have been uploaded...", BorderColor = "#3498DB" }, // Unread (Blue border)
            new MessageItem { Sender = "Admin Office", Time = "Yesterday", Subject = "Insurance Update", PreviewText = "Please review your updated policy documents.", BorderColor = "#E0E6ED" } // Read (Gray border)
        };
    }
}