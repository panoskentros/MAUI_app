namespace MAUI_app.View.interfaces;

public interface IRegisterView
{
    void SetUsernameError(string message, bool isVisible);
    void SetEmailError(string message, bool isVisible);
    void SetPasswordError(string message, bool isVisible);
    void SetConfirmPasswordError(string message, bool isVisible);
    void SetLoadingState(bool isLoading);
    void ClearInputs();
    void UpdatePasswordVisibilityState(bool isHidden);
    void UpdateConfirmPasswordVisibilityState(bool isHidden);
    Task ShowAlertAsync(string title, string message);
    Task NavigateBackAsync();
}