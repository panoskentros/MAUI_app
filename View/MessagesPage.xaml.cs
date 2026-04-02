using MAUI_app.Controller;
using MAUI_app.Services;

namespace MAUI_app.View;

public partial class MessagesPage : ContentPage
{
    private MessagesController _controller;
    private readonly IAuthService _authService;

    public MessagesPage(IAuthService authService)
    {
        InitializeComponent();
        _controller = new MessagesController();
        _authService = authService;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        if (_authService.CurrentUser != null)
        {
            PageBanner.SetTitle("Daily Schedule");
            PageBanner.SetWelcomeMessage(_authService.CurrentUser.UserName);
        }

        MessagesList.ItemsSource = _controller.GetInboxMessages();
    }
}