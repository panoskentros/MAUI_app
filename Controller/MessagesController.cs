using System.Collections.Generic;
using System.Threading.Tasks;
using MAUI_app.Model;
using MAUI_app.Data;
using MAUI_app.View.interfaces;

namespace MAUI_app.Controller;

public class MessagesController
{
    private readonly IMessagesView _view;

    public MessagesController(IMessagesView view)
    {
        _view = view;
    }

    public async Task InitializeDataAsync()
    {
        var messages = new List<MessageItem>
        {
            new MessageItem 
            { 
                Sender = "Dr. Sarah Jenkins", 
                Time = "10:30 AM", 
                Subject = "Lab Results are ready", 
                PreviewText = "Your recent blood work results have been uploaded...", 
                BorderColor = "#3498DB" 
            },
            new MessageItem 
            { 
                Sender = "Admin Office", 
                Time = "Yesterday", 
                Subject = "Insurance Update", 
                PreviewText = "Please review your updated policy documents.", 
                BorderColor = "#E0E6ED" 
            },
            new MessageItem 
            { 
                Sender = "Dr. Sarah Jenkins", 
                Time = "10:30 AM", 
                Subject = "Lab Results are ready", 
                PreviewText = "Your recent blood work results have been uploaded...", 
                BorderColor = "#3498DB" 
            }
        };

        var result = Result<List<MessageItem>>.Ok(messages, "Messages retrieved successfully.");

        if (result.Success)
        {
            _view.SetMessages(result.Data);
        }
        else
        {
            await _view.ShowErrorAsync(result.Message);
        }
    }
}