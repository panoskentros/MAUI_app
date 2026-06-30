using System;
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
    private readonly IUserService _userService;
    public static readonly BindableProperty CanRescheduleProperty =
        BindableProperty.Create(nameof(CanReschedule), typeof(bool), typeof(MyAppointmentsPage), false);
    
    public bool CanReschedule
    {
        get => (bool)GetValue(CanRescheduleProperty);
        set => SetValue(CanRescheduleProperty, value);
    }
    
    public MyAppointmentsPage(IAppointmentService appointmentService, IUserService userService)
    {
        InitializeComponent();
        _userService = userService;
        _controller = new AppointmentsController(this, appointmentService, userService);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_userService.CurrentUser != null)
        {
            AddAppointmentBtn.IsVisible = _userService.CurrentUser.Role != UserRole.Doctor;
            CanReschedule = _userService.CurrentUser.Role == UserRole.Patient || 
                            _userService.CurrentUser.Role == UserRole.Secretary;
        }

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
    
    private async void OnRescheduleAppointmentClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.CommandParameter is Appointment appointmentToReschedule)
        {
            var navigationParameter = new Dictionary<string, object>
            {
                { "AppointmentToEdit", appointmentToReschedule }
            };
            await Shell.Current.GoToAsync(nameof(BookAppointmentPage), navigationParameter);
        }
    }

    private async void OnCancelAppointmentClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.CommandParameter is Appointment appointmentToCancel)
        {
            bool confirm = await DisplayAlert("Cancel Appointment", "Are you sure you want to cancel this appointment?", "Yes", "No");
        
            if (confirm)
            {
              await _controller.CancelAppointmentAsync(appointmentToCancel);
            }
        }
    }

    private bool _isRefreshing = false;
    private async void OnRefreshClicked(object sender, EventArgs e)
    {
        if (_isRefreshing) return;

        try
        {
            _isRefreshing = true;
            await _controller.InitializeDataAsync();
        }
        finally
        {
            _isRefreshing = false;
        }
    }

    private async void OnAddAppointmentClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(BookAppointmentPage));
    }
}