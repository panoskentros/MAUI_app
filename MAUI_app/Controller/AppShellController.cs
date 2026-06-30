using System;
using System.Threading.Tasks;
using MAUI_app.Model;
using MAUI_app.Services.Interfaces;
using MAUI_app.View.interfaces;

namespace MAUI_app.Controller;

public class AppShellController
{
    private IAppShellView? _view;
    private readonly IUserService _userService;

    public AppShellController(IUserService userService)
    {
        _userService = userService;
    }

    public void SetView(IAppShellView view)
    {
        _view = view;
    }

    public void SubscribeToUserChanges()
    {
        _userService.UserChanged += UpdateMenuBasedOnRole;
    }

    public void UnsubscribeFromUserChanges()
    {
        _userService.UserChanged -= UpdateMenuBasedOnRole;
    }

    public void UpdateMenuBasedOnRole(object? sender, EventArgs e)
    {
        if (_userService.CurrentUser == null || _view == null) return;
        RefreshUserInfo();
    }

    public async Task HandleSignOutAsync()
    {
        try
        {
            _view?.CloseFlyout();
            _userService.Logout();
            
            if (_view != null)
            {
                await _view.NavigateToLoginAsync();
            }
        }
        catch (Exception ex)
        {
            _view?.ShowError(ex.Message);
        }
    }
    public void RefreshUserInfo()
    {
        var user = _userService.CurrentUser;
        if (user != null && _view != null)
            _view.UpdateUserDisplay(user.UserName);
    }
}