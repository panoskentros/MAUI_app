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
        // Navigate to the Add/Edit page
        await Shell.Current.GoToAsync(nameof(AddMedicationPage));
    }
    
    private async void OnMedicationTapped(object sender, TappedEventArgs e)
    {
        // 1. Έλεγχος Ασφαλείας: ΜΟΝΟ οι γιατροί μπορούν να κάνουν επεξεργασία!
        if (_userService.CurrentUser == null || _userService.CurrentUser.Role != UserRole.Doctor)
        {
            return; // Αν είναι ασθενής, σταματάει εδώ και δεν κάνει τίποτα.
        }

        // 2. Παίρνουμε τα δεδομένα της συγκεκριμένης κάρτας που πατήθηκε
        if (sender is Border border && border.BindingContext is Medication selectedMed)
        {
            // Κάνουμε ένα ωραίο εφέ στο κουμπί/κάρτα
            await border.ScaleTo(0.95, 100);
            await border.ScaleTo(1.0, 100);

            // 3. Στέλνουμε το συγκεκριμένο φάρμακο στη σελίδα προσθήκης/επεξεργασίας
            var navigationParameter = new Dictionary<string, object>
            {
                { "MedicationToEdit", selectedMed }
            };
            
            await Shell.Current.GoToAsync(nameof(AddMedicationPage), navigationParameter);
        }
    }
}