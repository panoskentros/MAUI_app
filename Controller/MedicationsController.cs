using MAUI_app.Model;
using MAUI_app.Services.Interfaces;

namespace MAUI_app.Controller;

public class MedicationsController : BaseController
{
    public MedicationsController(IUserService userService) : base(userService)
    {
    }
    
    public void InitializeAsync()
    {
        SetupBanner("Medical Records", true);
    }
    
    public List<Medication> GetActivePrescriptions()
    {
        return new List<Medication>
        {
            new Medication { Name = "Amoxicillin 500mg", Instructions = "Take 1 pill every 8 hours", Icon = "pill_icon.png" },
            new Medication { Name = "Lisinopril 20mg", Instructions = "Take 1 pill daily with food", Icon = "pill_icon.png" }
        };
    }
}