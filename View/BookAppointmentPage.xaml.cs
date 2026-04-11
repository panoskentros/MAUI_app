using MAUI_app.Controller;
using MAUI_app.Services.Interfaces;
using MAUI_app.Model;

namespace MAUI_app.View;

public partial class BookAppointmentPage : ContentPage
{
    private readonly BookAppointmentController _controller;
    private readonly IUserService _userService;

    public BookAppointmentPage(IUserService userService, IAppointmentService appointmentService)
    {
        InitializeComponent();
        _userService = userService;
        _controller = new BookAppointmentController(appointmentService, userService);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        var user = _userService.CurrentUser;
        if (user == null) return;

        PatientPickerContainer.IsVisible = user.Role == UserRole.Secretary || user.Role == UserRole.Doctor;
        
        DoctorPicker.ItemsSource = await _controller.GetDoctorsAsync();
        
        if (PatientPickerContainer.IsVisible)
        {
            PatientPicker.ItemsSource = await _controller.GetPatientsAsync();
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        var selectedDoctor = DoctorPicker.SelectedItem as ApplicationUser;
        var selectedPatient = PatientPicker.SelectedItem as ApplicationUser;
        var date = ApptDatePicker.Date;
        var time = ApptTimePicker.Time;
        var notes = NotesEditor.Text;

        var result = await _controller.SaveAppointmentAsync(selectedDoctor, selectedPatient, date, time, notes);

        if (result.Success)
        {
            await DisplayAlert("Success", result.Message, "OK");
            await Navigation.PopAsync();
        }
        else
        {
            await DisplayAlert("Error", result.Message, "OK");
        }
    }
}