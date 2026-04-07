using MAUI_app.Controller;

namespace MAUI_app.View;

public partial class MessagesPage : ContentPage
{
    private readonly MessagesController _controller;

    public MessagesPage(MessagesController controller)
    {
        InitializeComponent();
        _controller = controller;
        BindingContext = _controller;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _controller.InitializeAsync();
    }
}