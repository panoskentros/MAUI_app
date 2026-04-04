using MAUI_app.Controller;
using MAUI_app.Services;
using MAUI_app.Services.Interfaces;

namespace MAUI_app.View;

public partial class MessagesPage : ContentPage
{
    private MessagesController _controller;
    private readonly IUserService _userService;

    public MessagesPage(IUserService userService)
    {
        InitializeComponent();
        _controller = new MessagesController();
        _userService = userService;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        if (_userService.CurrentUser != null)
        {
            PageBanner.SetTitle("Daily Schedule");
            PageBanner.SetWelcomeMessage(_userService.CurrentUser.UserName);
        }

        MessagesList.ItemsSource = _controller.GetInboxMessages();
    }
}