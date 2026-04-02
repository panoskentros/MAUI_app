using MAUI_app.Controller;
using MAUI_app.Services;

namespace MAUI_app.View;

public partial class MedicationsPage : ContentPage
{
    private readonly MedicationsController _controller;
    private readonly IAuthService _authService;
    
    public MedicationsPage(MedicationsController controller, IAuthService authService)
    {
        InitializeComponent();
        
        _controller = controller;
        _authService = authService;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        var user = _authService.CurrentUser;
        if (user != null)
        {
            PageBanner.SetTitle("Medical Records");
            PageBanner.SetWelcomeMessage(user.UserName);
        }
        
        MedicationsList.ItemsSource = _controller.GetActivePrescriptions();
    }
}