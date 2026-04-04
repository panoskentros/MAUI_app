using MAUI_app.Controller;
using MAUI_app.Services;
using MAUI_app.Services.Interfaces;

namespace MAUI_app.View;

public partial class MedicationsPage : ContentPage
{
    private readonly MedicationsController _controller;
    private readonly IUserService _userService;
    
    public MedicationsPage(MedicationsController controller, IUserService userService)
    {
        InitializeComponent();
        
        _controller = controller;
        _userService = userService;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        var user = _userService.CurrentUser;
        if (user != null)
        {
            PageBanner.SetTitle("Medical Records");
            PageBanner.SetWelcomeMessage(user.UserName);
        }
        
        MedicationsList.ItemsSource = _controller.GetActivePrescriptions();
    }
}