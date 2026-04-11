using System.Threading.Tasks;
using MAUI_app.Controller;
using Microsoft.Maui.Controls;

namespace MAUI_app.View;

public partial class MyAppointmentsPage : ContentPage
{
    private readonly AppointmentsController _controller;

    public MyAppointmentsPage(AppointmentsController controller)
    {
        InitializeComponent();
        _controller = controller;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadAppointmentsAsync();
    }

    private async Task LoadAppointmentsAsync()
    {
        var result = await _controller.GetDashboardDataAsync(); 
        
        if (result.Success)
        {
            AllAppointmentsList.ItemsSource = result.Data;
        }
        else
        {
            await DisplayAlert("Error", result.Message, "OK");
        }
    }
}