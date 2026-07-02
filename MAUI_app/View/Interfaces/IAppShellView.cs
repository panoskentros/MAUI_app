namespace MAUI_app.View.interfaces;

public interface IAppShellView
{
    void CloseFlyout();
    Task NavigateToLoginAsync();
    void ShowError(string message);
    void UpdateUserDisplay(string userName, string userRole);
    void SetMedicationsVisibility(bool isVisible);
}