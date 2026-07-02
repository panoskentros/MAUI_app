using MAUI_app.Model;

namespace MAUI_app.View.Interfaces;

public interface IAddMedicationView
{
    void SetPatients(List<ApplicationUser> patients);
    void PrefillData(Medication medication, List<ApplicationUser> patients);
    void SetSubmitButtonText(string text);
    Task ShowAlertAsync(string title, string message);
    Task NavigateBackAsync();
}