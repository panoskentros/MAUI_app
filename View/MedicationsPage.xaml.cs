using MAUI_app.Controller;
using MAUI_app.Services;

namespace MAUI_app.View;

public partial class MedicationsPage : ContentPage
{
    private MedicationsController _controller;

    public MedicationsPage(IAuthService authService)
    {
        InitializeComponent();
        _controller = new MedicationsController();

        if (authService.CurrentUser != null)
        {
            PageBanner.SetTitle("Medical Records");
            PageBanner.SetWelcomeMessage(authService.CurrentUser.UserName);
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // MVC: Inject data from Controller into the View
        MedicationsList.ItemsSource = _controller.GetActivePrescriptions();
    }
}