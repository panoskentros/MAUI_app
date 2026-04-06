using MAUI_app.Controller;
using MAUI_app.View.Interfaces;

namespace MAUI_app.View;

public partial class MyAppointmentsPage : IAppointmentsView
{
    private readonly AppointmentsController _controller;

    public MyAppointmentsPage(AppointmentsController controller)
    {
        InitializeComponent();
        _controller = controller;
        BindingContext = _controller; 
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _controller.InitializeAsync();
    }
}