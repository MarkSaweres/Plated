using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plated.Core.Models;
using Plated.Core.Navigation;
using Plated.Core.Services;

namespace Plated.Core.ViewModels;

public partial class ProfileViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private readonly IAppNavigator _navigator;

    [ObservableProperty]
    private AppUser? currentUser;

    public ProfileViewModel(IAuthService authService, IAppNavigator navigator)
    {
        _authService = authService;
        _navigator = navigator;
        CurrentUser = _authService.CurrentUser;
        _authService.AuthStateChanged += (_, user) => CurrentUser = user;
    }

    [RelayCommand]
    private async Task SignOutAsync()
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        try
        {
            await _authService.SignOutAsync();
            _navigator.ShowLogin();
        }
        catch (Exception)
        {
            ErrorMessage = "Couldn't sign out. Please try again.";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
