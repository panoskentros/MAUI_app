using MAUI_app.Controller;

namespace MAUI_app.View;

public partial class MedicationsPage : ContentPage
{
    private readonly MedicationsController _controller;
    
    public MedicationsPage(MedicationsController controller)
    {
        InitializeComponent();
        _controller = controller;
        
        Title = "Medical Records";
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadData();
    }

    private void LoadData()
    {
        var result = _controller.GetActivePrescriptions();
        
        if (result.Success)
        {
            MedicationsList.ItemsSource = result.Data;
        }
        else
        {
            DisplayAlert("Error", result.Message, "OK");
        }
    }
}