using MAUI_app.Controller;
using MAUI_app.Services.Interfaces;
using MAUI_app.View.interfaces;
using MAUI_app.Model;

namespace MAUI_app.View;

public partial class DashboardPage : ContentPage, IDashboardView
{
    private readonly DashboardController _controller;

    public DashboardPage(IAppointmentService appointmentService, IUserService userService)
    {
        InitializeComponent();
        _controller = new DashboardController(this, appointmentService, userService);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _controller.InitializeDashboardAsync();
    }

    private async void OnBookAppointmentTapped(object sender, EventArgs e)
    {
        await AnimateBorder(sender);
        await _controller.HandleBookAppointmentClicked();
    }
    
    private async void OnViewAllAppointmentsClicked(object sender, EventArgs e) => await _controller.HandleViewAllAppointmentsClicked();
    private async void OnDoctorSeeMoreClicked(object sender, EventArgs e) => await _controller.HandleViewAllAppointmentsClicked();

    private async void OnMedicalRecordsTapped(object sender, TappedEventArgs e)
    {
        await AnimateBorder(sender);
        await _controller.HandleMedicalRecordsClicked();
    }
    
    private async void OnMyScheduleTapped(object sender, TappedEventArgs e)
    {
        await AnimateBorder(sender);
        await _controller.HandleViewAllAppointmentsClicked();
    }
    
    private async void OnPrescriptionsTapped(object sender, EventArgs e)
    {
        await AnimateBorder(sender);
        await NavigateToMedicationsAsync();
    }
    
    private async Task AnimateBorder(object sender)
    {
        if (sender is Border border)
        {
            await border.ScaleTo(0.95, 100);
            await border.ScaleTo(1.0, 100);
        }
    }
    public void ShowPatientView(bool isVisible) => PatientView.IsVisible = isVisible;
    public void ShowSecretaryView(bool isVisible) => SecretaryView.IsVisible = isVisible;
    public void ShowDoctorView(bool isVisible) => DoctorView.IsVisible = isVisible;

    public void SetPatientNextAppointment(string dateText, string detailsText)
    {
        PatientNextApptDateLabel.Text = dateText;
        PatientNextApptDetailsLabel.Text = detailsText;
    }

    public void SetPatientMoreAppointmentsButton(bool isVisible, string buttonText = "")
    {
        PatientMoreApptsButton.IsVisible = isVisible;
        if (isVisible) PatientMoreApptsButton.Text = buttonText;
    }

    public void SetSecretaryAppointmentsToday(string countText) => SecretaryApptsTodayLabel.Text = countText;

    public void SetDoctorNextPatient(string timeAndNameText, string detailsText)
    {
        DoctorNextPatientTimeLabel.Text = timeAndNameText;
        DoctorNextPatientDetailsLabel.Text = detailsText;
    }

    public void SetDoctorMorePatientsButton(bool isVisible, string buttonText = "")
    {
        DoctorMorePatientsButton.IsVisible = isVisible;
        if (isVisible) DoctorMorePatientsButton.Text = buttonText;
    }
    
    public void SetPatientRescheduleButton(bool isVisible) => PatientRescheduleButton.IsVisible = isVisible;
    
    private async void OnRescheduleClicked(object sender, EventArgs e)
    {
        await _controller.HandleRescheduleClicked();
    }
    
    private async void OnWriteDiagnosisClicked(object sender, EventArgs e)
    {
        await _controller.HandleWriteDiagnosisClicked();
    }

    public async Task NavigateToBookAppointmentAsync(Appointment appt = null)
    {
        var navigationParameter = new Dictionary<string, object>
        {
            { "AppointmentToEdit", appt ?? new Appointment() }
        };
    
        await Shell.Current.GoToAsync(nameof(BookAppointmentPage), navigationParameter);
    }

    public Task NavigateToAppointmentsAsync() => Shell.Current.GoToAsync("//appointments");
    public Task NavigateToMedicationsAsync() => Shell.Current.GoToAsync("//medications");
    public async Task NavigateToAddMedicationAsync(Medication med = null)
    {
        var navigationParameter = new Dictionary<string, object>
        {
            { "MedicationToEdit", med ?? new Medication() }
        };
    
        await Shell.Current.GoToAsync(nameof(AddMedicationPage), navigationParameter);
    }
    public Task NavigateToSettingsAsync() => Shell.Current.GoToAsync("//settings");
}