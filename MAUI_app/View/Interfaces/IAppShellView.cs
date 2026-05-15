namespace MAUI_app.View.interfaces;

public interface IAppShellView
{
    void SetScheduleVisibility(bool isVisible);
    void SetAvailabilityVisibility(bool isVisible);
    void CloseFlyout();
    Task NavigateToLoginAsync();
    void ShowError(string message);
}