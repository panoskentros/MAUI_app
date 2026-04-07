using MAUI_app.Model;
using MAUI_app.Services.Interfaces;

namespace MAUI_app.Controller;

public class MessagesController : BaseController
{
    public MessagesController(IUserService userService) : base(userService)
    {
    }

    public void InitializeAsync()
    {
        SetupBanner("Inbox", true);
    }

    public List<MessageItem> GetInboxMessages()
    {
        return new List<MessageItem>
        {
            new MessageItem { Sender = "Dr. Sarah Jenkins", Time = "10:30 AM", Subject = "Lab Results are ready", PreviewText = "Your recent blood work results have been uploaded...", BorderColor = "#3498DB" },
            new MessageItem { Sender = "Admin Office", Time = "Yesterday", Subject = "Insurance Update", PreviewText = "Please review your updated policy documents.", BorderColor = "#E0E6ED" },
            new MessageItem { Sender = "Dr. Sarah Jenkins", Time = "10:30 AM", Subject = "Lab Results are ready", PreviewText = "Your recent blood work results have been uploaded...", BorderColor = "#3498DB" }
        };
    }
}