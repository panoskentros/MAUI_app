using MAUI_app.Controller;

namespace MAUI_app.View;

public partial class MessagesPage : ContentPage
{
    private readonly MessagesController _controller;

    public MessagesPage(MessagesController controller)
    {
        InitializeComponent();
        _controller = controller;
        Title = "Inbox";
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadMessages();
    }

    private void LoadMessages()
    {
        var result = _controller.GetInboxMessages();
        
        if (result.Success)
        {
            MessagesList.ItemsSource = result.Data;
        }
        else
        {
            DisplayAlert("Error", result.Message, "OK");
        }
    }
}