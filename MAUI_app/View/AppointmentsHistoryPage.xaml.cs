using MAUI_app.Controller;
using MAUI_app.Model;
using MAUI_app.Services.Interfaces;
using MAUI_app.View.interfaces;

namespace MAUI_app.View;

public partial class AppointmentsHistoryPage : ContentPage, IAppointmentsHistoryView
{
    private readonly AppointmentsHistoryController _controller;

    public AppointmentsHistoryPage(IAppointmentService appointmentService, IUserService userService)
    {
        InitializeComponent();
        _controller = new AppointmentsHistoryController(this, appointmentService, userService);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _controller.LoadHistoryAsync();
    }

    public void SetHistoryAppointments(IEnumerable<IGrouping<string, Appointment>> groupedAppointments)
    {
        HistoryList.ItemsSource = groupedAppointments;
    }
}