using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MAUI_app.Controller;
using MAUI_app.Model;
using MAUI_app.Services.Interfaces;
using MAUI_app.View.interfaces;

namespace MAUI_app.View;

public partial class MyAppointmentsPage : ContentPage, IAppointmentsView
{
    private readonly AppointmentsController _controller;

    public MyAppointmentsPage(IAppointmentService appointmentService, IUserService userService)
    {
        InitializeComponent();
        _controller = new AppointmentsController(this, appointmentService, userService);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _controller.InitializeDataAsync();
    }
    public void SetAppointments(List<Appointment> appointments)
    {
        AllAppointmentsList.ItemsSource = appointments;
    }
    public Task ShowErrorAsync(string message)
    {
        return DisplayAlert("Error", message, "OK");
    }
}