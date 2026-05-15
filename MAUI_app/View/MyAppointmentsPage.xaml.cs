using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public void SetAppointments(ObservableCollection<Appointment> appointments)
    {
        AllAppointmentsList.ItemsSource = appointments;
    }
    
    public Task ShowMessageAsync(string message, bool isError = false)
    {
        if (isError)
            return DisplayAlert("Error", message, "OK");
        else
            return DisplayAlert("Info", message, "OK");
    }

 
    private async void OnCancelAppointmentClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Appointment appointmentToCancel)
        {
            bool confirm = await DisplayAlert("Cancel Appointment", "Are you sure you want to cancel this appointment?", "Yes", "No");
        
            if (confirm)
            {
              await _controller.CancelAppointmentAsync(appointmentToCancel);
            }
        }
    }
}