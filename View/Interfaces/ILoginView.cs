using MAUI_app.Model;

namespace MAUI_app.View.Interfaces;

public interface ILoginView
{
    void SetLoading(bool isLoading);
    Task ShowAlert(string title, string message);
    Task NavigateToDashboard();
    Task NavigateToRegister();
    void ClearFields();
}