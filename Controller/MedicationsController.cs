using MAUI_app.Model;
using MAUI_app.Data;

namespace MAUI_app.Controller;

public class MedicationsController
{
    public MedicationsController()
    {
    }
    
    public Result<List<Medication>> GetActivePrescriptions()
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

        return Result<List<Medication>>.Ok(medications, "Prescriptions retrieved successfully.");
    }
}