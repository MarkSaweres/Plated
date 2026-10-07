using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plated.Core.Navigation;
using Plated.Core.Services;

namespace Plated.Core.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private readonly IAppNavigator _navigator;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    public LoginViewModel(IAuthService authService, IAppNavigator navigator)
    {
        _authService = authService;
        _navigator = navigator;
    }

    [RelayCommand]
    private Task SignInWithGoogleAsync()
        => RunAuthAsync(() => _authService.SignInWithGoogleAsync(), "Google sign-in failed. Please try again.");

    [RelayCommand]
    private async Task SignInWithEmailAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || !Email.Contains('@'))
        {
            ErrorMessage = "Enter a valid email address.";
            return;
        }

        if (string.IsNullOrEmpty(Password))
        {
            ErrorMessage = "Enter your password.";
            return;
        }

        await RunAuthAsync(() => _authService.SignInWithEmailAsync(Email, Password), "Couldn't sign in. Please try again.");
    }

    [RelayCommand]
    private Task GoToCreateAccountAsync()
    {
        ErrorMessage = null;
        return _navigator.ShowCreateAccountAsync();
    }

    private async Task RunAuthAsync(Func<Task> action, string failureMessage)
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        ErrorMessage = null;
        try
        {
            await action();
            _navigator.ShowMainApp();
        }
        catch (OperationCanceledException)
        {
            // The person closed the sign-in sheet; nothing to report.
        }
        catch (Exception ex)
        {
            ShowAuthError(ex, failureMessage);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
