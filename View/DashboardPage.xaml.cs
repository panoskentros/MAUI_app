using MAUI_app.Model;
using MAUI_app.Services;
using MAUI_app.Controller;

namespace MAUI_app.View;

public partial class DashboardPage : ContentPage
{
    private readonly IAuthService _authService;
    private readonly DashboardController _controller;

    public DashboardPage(IAuthService authService, DashboardController controller)
    {
        InitializeComponent();
        _authService = authService;
        _controller = controller;
        BindingContext = _controller;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        SetupDashboardBasedOnRole();
        await _controller.InitializeAsync();
    }

    private void SetupDashboardBasedOnRole()
    {
        var user = _authService.CurrentUser;
        if (user == null) return;

        PatientView.IsVisible = false;
        SecretaryView.IsVisible = false;
        DoctorView.IsVisible = false;

        if (user.Role == UserRole.Patient)
        {
            PatientView.IsVisible = true;
            
            PageBanner.SetTitle("Patient Dashboard");
            PageBanner.SetWelcomeMessage(user.UserName); 
        }
        else if (user.Role == UserRole.Secretary)
        {
            SecretaryView.IsVisible = true;
            
            PageBanner.SetTitle("Clinic Control Center");
            PageBanner.SetWelcomeMessage(user.UserName);
        }
        else if (user.Role == UserRole.Doctor)
        {
            DoctorView.IsVisible = true;
            
            PageBanner.SetTitle("Doctor Dashboard");
            PageBanner.SetWelcomeMessage("Dr. " + user.UserName);
        }
    }
    
    private async void OnBookAppointmentTapped(object sender, TappedEventArgs e)
    {
        if (sender is Border border)
        {
            await border.ScaleTo(0.95, 100);
            await border.ScaleTo(1.0, 100);
        }
        
        await Shell.Current.GoToAsync("//appointments");
    }
    
    private async void OnViewAllAppointmentsClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//appointments");
    }

    private async void OnMedicalRecordsTapped(object sender, TappedEventArgs e)
    {
        if (sender is Border border)
        {
            await border.ScaleTo(0.95, 100);
            await border.ScaleTo(1.0, 100);
        }
        
        await Shell.Current.GoToAsync("//medications");
    }
}