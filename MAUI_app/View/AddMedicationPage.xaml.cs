using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MAUI_app.Controller;
using MAUI_app.Model;
using MAUI_app.Services.Interfaces;
using MAUI_app.View.Interfaces;

namespace MAUI_app.View;

[QueryProperty(nameof(MedicationToEdit), "MedicationToEdit")]
public partial class AddMedicationPage : ContentPage, IAddMedicationView
{
    private readonly AddMedicationController _controller;
    private Medication _medicationToEdit;
    
    public Medication MedicationToEdit
    {
        get => _medicationToEdit;
        set
        {
            _medicationToEdit = value;
            _controller.SetMedicationToEdit(value);
        }
    }

    public AddMedicationPage(IUserService userService, MedicationsController medicationsController)
    {
        InitializeComponent();
        
        // Assuming a dedicated controller is created, similar to BookAppointmentController
        _controller = new AddMedicationController(this, medicationsController, userService);
        
        // Default the End Date to +7 days to save the doctor time
        EndDatePicker.Date = DateTime.Now.AddDays(7);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _controller.OnViewAppearing();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        await _controller.SaveMedication(
            PatientPicker.SelectedItem as ApplicationUser,
            MedicationNameEntry.Text,
            MedicationInstructionsEditor.Text,
            StartDatePicker.Date,
            EndDatePicker.Date);
    }
    
    public void PrefillData(Medication medication, List<ApplicationUser> patients)
    {
        MedicationNameEntry.Text = medication.MedicationName;
        MedicationInstructionsEditor.Text = medication.Instructions;
        StartDatePicker.Date = medication.StartDate.Date;
        EndDatePicker.Date = medication.EndDate.Date;
        
        if (patients != null)
            PatientPicker.SelectedItem = patients.FirstOrDefault(p => p.Id == medication.ApplicationUserId);
    }

    public void SetSubmitButtonText(string text)
    {
        SubmitButton.Text = text;
    }

    public void SetPatients(List<ApplicationUser> patients)
    {
        PatientPicker.ItemsSource = patients;
    }

    public Task ShowAlertAsync(string title, string message) => DisplayAlert(title, message, "OK");
    
    public Task NavigateBackAsync() => Navigation.PopAsync();
}