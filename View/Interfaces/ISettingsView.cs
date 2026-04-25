namespace MAUI_app.View.interfaces;

public interface ISettingsView
{
    void SetNotificationsToggleState(bool isEnabled);
    Task ShowErrorAsync(string message);
    Task NavigateToLoginAsync();
}