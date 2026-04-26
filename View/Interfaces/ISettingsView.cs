using System.Threading.Tasks;

namespace MAUI_app.View.interfaces;

public interface ISettingsView
{
    void SetUserData(string username, string email);
    Task ShowMessageAsync(string message, bool isError = false);
    Task NavigateToLoginAsync();
}