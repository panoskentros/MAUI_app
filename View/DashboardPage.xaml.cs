using MAUI_app.Controller;
using MAUI_app.Model;

namespace MAUI_app.View;

public partial class DashboardPage : ContentPage
{
    private readonly DashboardController _controller;

    public DashboardPage(DashboardController controller)
    {
        InitializeComponent();
        _controller = controller;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadDashboardDataAsync();
    }

    private async Task LoadDashboardDataAsync()
    {
        var result = await _controller.GetDashboardDataAsync();
        
        if (!result.Success)
        {
            return;
        }

        var data = result.Data;

        PatientView.IsVisible = data.Role == UserRole.Patient;
        SecretaryView.IsVisible = data.Role == UserRole.Secretary;
        DoctorView.IsVisible = data.Role == UserRole.Doctor;

        if (data.Role == UserRole.Patient)
        {
            if (data.Appointments != null && data.Appointments.Any())
            {
                var nextAppt = data.Appointments.First();
                PatientNextApptDateLabel.Text = nextAppt.AppointmentDate.ToString("dddd, MMM dd - h:mm tt");
                PatientNextApptDetailsLabel.Text = "Reason: " + (string.IsNullOrWhiteSpace(nextAppt.MedicalNotes) ? "General Checkup" : nextAppt.MedicalNotes);
                
                bool hasMore = data.Appointments.Count > 1;
                PatientMoreApptsButton.IsVisible = hasMore;
                if (hasMore)
                {
                    PatientMoreApptsButton.Text = $"See {data.Appointments.Count - 1} More";
                }
            }
            else
            {
                PatientNextApptDateLabel.Text = "No upcoming appointments";
                PatientNextApptDetailsLabel.Text = "Book a new appointment below";
                PatientMoreApptsButton.IsVisible = false;
            }
        }
        else if (data.Role == UserRole.Secretary)
        {
            SecretaryApptsTodayLabel.Text = data.TodaysCount.ToString();
        }
        else if (data.Role == UserRole.Doctor)
        {
            if (data.Appointments != null && data.Appointments.Any())
            {
                var nextPatient = data.Appointments.First();
                DoctorNextPatientTimeLabel.Text = nextPatient.AppointmentDate.ToString("h:mm tt") + " - " + nextPatient.PatientName;
                DoctorNextPatientDetailsLabel.Text = "Reason: " + (string.IsNullOrWhiteSpace(nextPatient.MedicalNotes) ? "Standard Checkup" : nextPatient.MedicalNotes);

                bool hasMore = data.Appointments.Count > 1;
                DoctorMorePatientsButton.IsVisible = hasMore;
                if (hasMore)
                {
                    DoctorMorePatientsButton.Text = $"See {data.Appointments.Count - 1} More Today";
                }
            }
            else
            {
                DoctorNextPatientTimeLabel.Text = "No more patients today";
                DoctorNextPatientDetailsLabel.Text = "Enjoy your break!";
                DoctorMorePatientsButton.IsVisible = false;
            }
        }
    }

    private async void OnBookAppointmentTapped(object sender, EventArgs e)
    {
        if (sender is Border border)
        {
            await border.ScaleTo(0.95, 100);
            await border.ScaleTo(1.0, 100);
        }
        
        var bookPage = Handler.MauiContext.Services.GetService<BookAppointmentPage>();
        await Navigation.PushAsync(bookPage);
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