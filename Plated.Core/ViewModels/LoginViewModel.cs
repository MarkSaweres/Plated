using CommunityToolkit.Mvvm.Input;
using Plated.Core.Navigation;
using Plated.Core.Services;

namespace Plated.Core.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private readonly IAppNavigator _navigator;

    public LoginViewModel(IAuthService authService, IAppNavigator navigator)
    {
        _authService = authService;
        _navigator = navigator;
    }

    [RelayCommand]
    private async Task SignInWithGoogleAsync()
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        ErrorMessage = null;
        try
        {
            await _authService.SignInWithGoogleAsync();
            _navigator.ShowMainApp();
        }
        catch (Exception)
        {
            ErrorMessage = "Sign-in failed. Please try again.";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
