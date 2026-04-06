using MAUI_app.Controller;
using MAUI_app.Model;
using MAUI_app.Services;
using MAUI_app.Services.Interfaces;

namespace MAUI_app.View;

public partial class SettingsPage : ContentPage
{
    private SettingsController _controller;

    public SettingsPage(SettingsController controller)
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