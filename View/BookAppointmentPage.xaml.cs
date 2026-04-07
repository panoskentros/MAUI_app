using MAUI_app.Controller;
using MAUI_app.Services.Interfaces;
using MAUI_app.View.Interfaces;

namespace MAUI_app.View;

public partial class BookAppointmentPage  : IBookAppointmentView
{
    private readonly BookAppointmentController _controller;
    private IBookAppointmentView bookAppointmentView;

    public BookAppointmentPage(IUserService _userService,IAppointmentService _appointmentService)
    {
        InitializeComponent();
        
        _controller = new BookAppointmentController(this, _appointmentService,_userService);
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
    public async Task ShowAlert(string title, string message,string cancelMsg="OK")
    {
        await DisplayAlert(title, message, cancelMsg);
    }
}