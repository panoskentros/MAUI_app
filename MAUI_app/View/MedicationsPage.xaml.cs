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

    public bool IsDoctor { get; set; }

    public MedicationsPage(MedicationsController controller, IUserService userService)
    {
        InitializeComponent();
        _controller = controller;
        _userService = userService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        if (_userService.CurrentUser != null)
        {
            IsDoctor = _userService.CurrentUser.Role == UserRole.Doctor;
            AddMedicationButton.IsVisible = IsDoctor;
        }

        await LoadData();
    }

    private async Task LoadData()
    {
        if (_userService.CurrentUser != null)
        {
            var medications = await _controller.GetMedicationsAsync(_userService.CurrentUser);
            
            foreach (var med in medications)
            {
                if (_userService.CurrentUser.Role == UserRole.Doctor || _userService.CurrentUser.Role == UserRole.Secretary)
                {
                    med.DisplayName = med.Patient?.UserName ?? "Unknown";
                }
                else
                {
                    med.DisplayName = "Dr. " + (med.Doctor?.UserName ?? "Unknown");
                }
            }

            MedicationsCollectionView.ItemsSource = new ObservableCollection<Medication>(medications);
        }
    }

    private async void OnAddMedicationClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AddMedicationPage));
    }
    
    private async void OnEditMedicationClicked(object sender, EventArgs e)
    {
        if (_userService.CurrentUser == null || _userService.CurrentUser.Role != UserRole.Doctor)
        {
            return; 
        }
        
        if (sender is ImageButton button && button.CommandParameter is Medication selectedMed)
        {
            await button.ScaleTo(0.90, 100);
            await button.ScaleTo(1.0, 100);
            
            var navigationParameter = new Dictionary<string, object>
            {
                { "MedicationToEdit", selectedMed }
            };
            
            await Shell.Current.GoToAsync(nameof(AddMedicationPage), navigationParameter);
        }
    }
}