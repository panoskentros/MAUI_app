using MAUI_app.Model;

namespace MAUI_app.Controller;

public class MedicationsController
{
    public List<Medication> GetActivePrescriptions()
    {
        return new List<Medication>
        {
            new Medication { Name = "Amoxicillin 500mg", Instructions = "Take 1 pill every 8 hours", Icon = "pill_icon.png" },
            new Medication { Name = "Lisinopril 20mg", Instructions = "Take 1 pill daily with food", Icon = "pill_icon.png" }
        };
    }
}