using MAUI_app.Controller;
using MAUI_app.Services;

namespace MAUI_app.View;

public partial class MessagesPage : ContentPage
{
    private MessagesController _controller;

    public MessagesPage(IAuthService authService)
    {
        InitializeComponent();
        _controller = new MessagesController();

        if (authService.CurrentUser != null)
        {
            PageBanner.SetWelcomeMessage(authService.CurrentUser.UserName);
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // MVC: Inject data from Controller into the View
        MessagesList.ItemsSource = _controller.GetInboxMessages();
    }
}