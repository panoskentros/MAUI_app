using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MAUI_app.Controller;
using MAUI_app.Model;
using MAUI_app.Services.Interfaces;
using MAUI_app.View.interfaces;

namespace MAUI_app.View;

public partial class BookAppointmentPage : ContentPage, IBookAppointmentView
{
    private readonly BookAppointmentController _controller;

    public BookAppointmentPage(IUserService userService, IAppointmentService appointmentService)
    {
        InitializeComponent();
        _controller = new BookAppointmentController(this, appointmentService, userService);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _controller.OnViewAppearing();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        await _controller.SaveAppointment(
            DoctorPicker.SelectedItem as ApplicationUser,
            PatientPicker.SelectedItem as ApplicationUser,
            ApptDatePicker.Date,
            ApptTimePicker.Time,
            NotesEditor.Text);
    }

    public void SetDoctors(List<ApplicationUser> doctors) => DoctorPicker.ItemsSource = doctors;
    public void SetPatients(List<ApplicationUser> patients) => PatientPicker.ItemsSource = patients;
    public void ShowPatientSelection(bool isVisible) => PatientPickerContainer.IsVisible = isVisible;
    public Task ShowAlertAsync(string title, string message) => DisplayAlert(title, message, "OK");
    public Task NavigateBackAsync() => Navigation.PopAsync();
}