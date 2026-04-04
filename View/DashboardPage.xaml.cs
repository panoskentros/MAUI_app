using MAUI_app.Model;
using MAUI_app.Services;
using MAUI_app.Controller;
using MAUI_app.Services.Interfaces;

namespace MAUI_app.View;

public partial class DashboardPage : ContentPage
{
    private readonly DashboardController _controller;

    public DashboardPage(DashboardController controller)
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
    
    private async void OnDoctorSeeMoreClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//appointments");
    }

    private async void OnMyScheduleTapped(object sender, TappedEventArgs e)
    {
        if (sender is Border border)
        {
            await border.ScaleTo(0.95, 100);
            await border.ScaleTo(1.0, 100);
        }
        await Shell.Current.GoToAsync("//appointments");
    }

    private async void OnSetAvailabilityTapped(object sender, TappedEventArgs e)
    {
        if (sender is Border border)
        {
            await border.ScaleTo(0.95, 100);
            await border.ScaleTo(1.0, 100);
        }
        await Shell.Current.GoToAsync("//settings");
    }
}