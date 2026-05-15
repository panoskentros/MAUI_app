using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MAUI_app.Controller;
using MAUI_app.Services.Interfaces;
using MAUI_app.View.interfaces;

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

    private async void OnSetAvailabilityTapped(object sender, TappedEventArgs e)
    {
        await AnimateBorder(sender);
        await _controller.HandleSettingsClicked();
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

    public async Task NavigateToBookAppointmentAsync()
    {
        var bookPage = Handler.MauiContext.Services.GetService<BookAppointmentPage>();
        await Navigation.PushAsync(bookPage);
    }

    public Task NavigateToAppointmentsAsync() => Shell.Current.GoToAsync("//appointments");
    public Task NavigateToMedicationsAsync() => Shell.Current.GoToAsync("//medications");
    public Task NavigateToSettingsAsync() => Shell.Current.GoToAsync("//settings");
}