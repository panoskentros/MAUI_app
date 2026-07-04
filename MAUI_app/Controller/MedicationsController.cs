using System.Collections.Generic;
using System.Threading.Tasks;
using MAUI_app.Model;
using MAUI_app.Services.Interfaces;

namespace MAUI_app.Controller;

public class MedicationsController
{
    private readonly IMedicationService _medicationService;

    public MedicationsController(IMedicationService medicationService)
    {
        _medicationService = medicationService;
    }
    
    public Task<List<Medication>> GetMedicationsAsync(ApplicationUser currentUser)
    {
        return _medicationService.GetMedicationsAsync(currentUser);
    }

    public Task<bool> SaveMedicationAsync(Medication medication, ApplicationUser currentUser)
    {
        return _medicationService.SaveMedicationAsync(medication, currentUser);
    }
    
    public Task<bool> DeleteMedicationAsync(int medicationId)
    {
        return _medicationService.DeleteMedicationAsync(medicationId);
    }
}