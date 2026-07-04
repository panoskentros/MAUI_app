using MAUI_app.Model;

namespace MAUI_app.Services.Interfaces;

public interface IMedicationService
{
    Task<List<Medication>> GetMedicationsAsync(ApplicationUser currentUser);
    Task<bool> SaveMedicationAsync(Medication medication, ApplicationUser currentUser);
    Task<bool> DeleteMedicationAsync(int medicationId);
}