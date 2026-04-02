namespace MAUI_app.View.Interfaces;

public interface ILoginView
{
    void SetLoading(bool isLoading);
    Task ShowAlert(string title, string message);
    Task NavigateToDashboard();
    Task NavigateToRegister();
    void ClearFields();
    void ClearErrors();
    void ShowFieldError(string propertyName, string errorMessage);
}