using MAUI_app.Controller;
using MAUI_app.Model;
using MAUI_app.View.Interfaces;
using MAUI_app.Services.Interfaces;
using System.Collections.ObjectModel;

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

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        // Show/Hide Add button based on Role
        if (_userService.CurrentUser != null)
        {
            AddMedicationButton.IsVisible = _userService.CurrentUser.Role == UserRole.Doctor;
        }

        await LoadData();
    }

    private async Task LoadData()
    {
        if (_userService.CurrentUser != null)
        {
            var medications = await _controller.GetMedicationsAsync(_userService.CurrentUser);
            MedicationsCollectionView.ItemsSource = new ObservableCollection<Medication>(medications);
        }
    }

    private async void OnAddMedicationClicked(object sender, EventArgs e)
    {
        // Navigate to the Add/Edit page
        await Shell.Current.GoToAsync(nameof(AddMedicationPage));
    }
}