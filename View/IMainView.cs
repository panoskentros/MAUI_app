using MAUI_app.Model;

namespace MAUI_app.View;

public interface IMainView
{
    void SetLoading(bool isLoading);
    Task ShowAlert(string title, string message);
    Task NavigateToDashboard(ApplicationUser user);
    Task NavigateToRegister();
    void ClearFields();
}