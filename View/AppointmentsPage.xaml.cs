using MAUI_app.Controller;
using MAUI_app.Services;

namespace MAUI_app.View;

public partial class AppointmentsPage : ContentPage
{
    private AppointmentsController _controller;

    // Inject IAuthService exactly like you did on the Dashboard!
    public AppointmentsPage(IAuthService authService)
    {
        InitializeComponent();
        _controller = new AppointmentsController(); 
        
        // Pass the username to the banner!
        if (authService.CurrentUser != null)
        {
            PageBanner.SetWelcomeMessage(authService.CurrentUser.UserName);
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadDynamicUI();
    }

    private void LoadDynamicUI()
    {
        string role = _controller.GetCurrentUserRole();

        if (role == "Patient")
        {
            PatientViewContainer.IsVisible = true;
            DoctorViewContainer.IsVisible = false;
            PatientAppointmentsList.ItemsSource = _controller.GetPatientAppointments();
        }
        else if (role == "Doctor")
        {
            PatientViewContainer.IsVisible = false;
            DoctorViewContainer.IsVisible = true;
            DoctorScheduleList.ItemsSource = _controller.GetDoctorSchedule();
        }
    }
}