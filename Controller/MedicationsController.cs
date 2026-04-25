using System.Collections.Generic;
using System.Threading.Tasks;
using MAUI_app.Model;
using MAUI_app.Data;
using MAUI_app.View.interfaces;

namespace MAUI_app.Controller;

public class MedicationsController
{
    private readonly IMedicationsView _view;

    public MedicationsController(IMedicationsView view)
    {
        _view = view;
    }
    
    public async Task InitializeDataAsync()
    {
        var medications = new List<Medication>
        {
            new Medication 
            { 
                Name = "Amoxicillin 500mg", 
                Instructions = "Take 1 pill every 8 hours", 
                Icon = "pill_icon.png" 
            },
            new Medication 
            { 
                Name = "Lisinopril 20mg", 
                Instructions = "Take 1 pill daily with food", 
                Icon = "pill_icon.png" 
            }
        };

        var result = Result<List<Medication>>.Ok(medications, "Prescriptions retrieved successfully.");

        if (result.Success)
        {
            _view.SetMedications(result.Data);
        }
        else
        {
            await _view.ShowErrorAsync(result.Message);
        }
    }
}