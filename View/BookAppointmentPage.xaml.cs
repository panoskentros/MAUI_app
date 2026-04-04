using MAUI_app.Controller;
using MAUI_app.Data;
using MAUI_app.Model;
using MAUI_app.Services;
using MAUI_app.Services.Interfaces;
using MAUI_app.View.Interfaces;

namespace MAUI_app.View;

public partial class BookAppointmentPage  : ContentPage, IAppointmentsView
{
    private readonly BookAppointmentController _controller;
    private readonly IUserService _userService;

    public BookAppointmentPage(BookAppointmentController controller, IUserService userService)
    {
        InitializeComponent();
        
        _controller = controller;
        _userService = userService;
        BindingContext = _controller; 
    }
    
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _controller.InitializeAsync();
    }
    
    private async void OnSaveClicked(object sender, EventArgs e)
    {
        bool success = await _controller.SaveAppointmentAsync();
        
        if (success)
        {
            await DisplayAlert("Success", "Appointment booked successfully!", "OK");
            await Navigation.PopAsync(); 
        }
        else
        {
            await DisplayAlert("Error", "Please make sure you have selected a Doctor", "OK");
        }
    }
}