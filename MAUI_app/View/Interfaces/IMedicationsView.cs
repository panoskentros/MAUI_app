using MAUI_app.Model;

namespace MAUI_app.View.interfaces;

public interface IMedicationsView
{
    void SetMedications(List<Medication> medications);
    Task ShowErrorAsync(string message);
}