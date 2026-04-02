using MAUI_app.Controller;
using MAUI_app.Model;
using MAUI_app.Services;

namespace MAUI_app.View;

public partial class DashboardPage : ContentPage
{
    private DashboardController _controller;

    public DashboardPage(IAuthService authService)
    {
        InitializeComponent();
        _controller = new DashboardController();
        
        if (authService.CurrentUser != null)
        {
            PageBanner.SetWelcomeMessage(authService.CurrentUser.UserName);
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadDashboardData();
    }

    private void LoadDashboardData()
    {
        string role = _controller.GetCurrentUserRole();

        if (role == "Patient")
        {
            PatientDashboardContainer.IsVisible = true;
            DoctorDashboardContainer.IsVisible = false;

            // Fill Patient Data
            PatientNextApptLabel.Text = _controller.GetNextAppointmentPatient();
            UnreadMessagesLabel.Text = _controller.GetUnreadMessagesCount().ToString();
        }
        else if (role == "Doctor")
        {
            PatientDashboardContainer.IsVisible = false;
            DoctorDashboardContainer.IsVisible = true;

            // Fill Doctor Data
            DoctorStatsLabel.Text = _controller.GetTodayStatsDoctor();
            DoctorNextPatientLabel.Text = _controller.GetNextPatient();
        }
    }

    // A cool trick: Make the "Book" button jump to the Appointments tab!
    private async void OnBookAppointmentClicked(object sender, EventArgs e)
    {
        // "//appointments" is the Route you defined in AppShell.xaml
        await Shell.Current.GoToAsync("//appointments"); 
    }
}