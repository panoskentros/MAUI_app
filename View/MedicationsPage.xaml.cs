using MAUI_app.Controller;

namespace MAUI_app.View;

public partial class MedicationsPage : ContentPage
{
    private readonly MedicationsController _controller;
    
    public MedicationsPage(MedicationsController controller)
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