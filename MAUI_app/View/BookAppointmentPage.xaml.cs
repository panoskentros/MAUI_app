using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MAUI_app.Controller;
using MAUI_app.Model;
using MAUI_app.Services.Interfaces;
using MAUI_app.View.interfaces;

namespace MAUI_app.View;

[QueryProperty(nameof(AppointmentToEdit), "AppointmentToEdit")]
public partial class BookAppointmentPage : ContentPage, IBookAppointmentView
{
    private readonly BookAppointmentController _controller;
    private Appointment _appointmentToEdit;
    
    public Appointment AppointmentToEdit
    {
        get => _appointmentToEdit;
        set
        {
            _appointmentToEdit = value;
            _controller.SetAppointmentToEdit(value);
        }
    }

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
            NotesEditor.Text,
            MedicationNameEntry.Text,
            MedicationInstructionsEditor.Text);
    }
    
    public void PrefillData(Appointment appt, List<ApplicationUser> doctors, List<ApplicationUser> patients)
    {
        ApptDatePicker.Date = appt.AppointmentDate.Date;
        ApptTimePicker.Time = appt.AppointmentDate.TimeOfDay;
        NotesEditor.Text = appt.MedicalNotes;
        
        if (doctors != null)
            DoctorPicker.SelectedItem = doctors.FirstOrDefault(d => d.Id == appt.DoctorId);

        if (patients != null)
            PatientPicker.SelectedItem = patients.FirstOrDefault(p => p.Id == appt.ApplicationUserId);
    }

    public void SetSubmitButtonText(string text)
    {
        SubmitButton.Text = text;
    }

    public void SetDoctors(List<ApplicationUser> doctors) => DoctorPicker.ItemsSource = doctors;
    public void SetPatients(List<ApplicationUser> patients) => PatientPicker.ItemsSource = patients;
    public void ShowPatientSelection(bool isVisible) => PatientPickerContainer.IsVisible = isVisible;
    
    public void SetMedicationEditState(bool canEdit)
    {
        MedicationNameEntry.IsReadOnly = !canEdit;
        MedicationInstructionsEditor.IsReadOnly = !canEdit;
    }

    public Task ShowAlertAsync(string title, string message) => DisplayAlert(title, message, "OK");
    public Task NavigateBackAsync() => Navigation.PopAsync();
}