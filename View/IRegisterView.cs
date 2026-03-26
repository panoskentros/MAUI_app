namespace MAUI_app.View;

public interface IRegisterView
{
    void SetLoading(bool isLoading);
    Task ShowAlert(string title, string message);
    Task NavigateBack();
    string GetConfirmPassword();
}