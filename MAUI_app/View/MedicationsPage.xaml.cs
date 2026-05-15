using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MAUI_app.Controller;
using MAUI_app.Model;
using MAUI_app.View.interfaces;

namespace MAUI_app.View;

public partial class MedicationsPage : ContentPage, IMedicationsView
{
    private readonly MedicationsController _controller;
    
    public MedicationsPage()
    {
        InitializeComponent();
        
        _controller = new MedicationsController(this);
        Title = "Medical Records";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _controller.InitializeDataAsync();
    }

    public void SetMedications(List<Medication> medications)
    {
        MedicationsList.ItemsSource = medications;
    }

    public Task ShowErrorAsync(string message)
    {
        return DisplayAlert("Error", message, "OK");
    }
}