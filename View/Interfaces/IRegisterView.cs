namespace MAUI_app.View.Interfaces;

public interface IRegisterView
{
    void SetLoading(bool isLoading);
    Task ShowAlert(string title, string message);
    Task NavigateBack();
    string GetConfirmPassword();
    void ClearFields();
    void ClearErrors();
    void ShowFieldError(string propertyName, string errorMessage);
}