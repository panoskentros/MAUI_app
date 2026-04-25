using MAUI_app.Model;

namespace MAUI_app.View.interfaces;

public interface IMessagesView
{
    void SetMessages(List<MessageItem> messages);
    Task ShowErrorAsync(string message);
}