namespace MAUI_app.View.interfaces;

public interface ILoginView
{
    void SetUsernameError(string message, bool isVisible);
    void SetPasswordError(string message, bool isVisible);
    void SetLoadingState(bool isLoading);
    void ClearInputs();
    void UpdatePasswordVisibilityState(bool isHidden);
    Task NavigateToDashboardAsync();
    Task NavigateToRegisterAsync();
    void QuitApplication();
}