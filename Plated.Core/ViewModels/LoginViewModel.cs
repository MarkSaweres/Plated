using System.Diagnostics;
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
        => RunAuthAsync(
            () => _authService.SignInWithGoogleAsync(),
            "Google sign-in failed. Please try again.");

    [RelayCommand]
    private Task SignInWithEmailAsync()
        => RunEmailAuthAsync(() => _authService.SignInWithEmailAsync(Email, Password));

    [RelayCommand]
    private Task CreateAccountAsync()
        => RunEmailAuthAsync(() => _authService.CreateAccountWithEmailAsync(Email, Password));

    private async Task RunEmailAuthAsync(Func<Task> action)
    {
        if (string.IsNullOrWhiteSpace(Email) || !Email.Contains('@'))
        {
            ErrorMessage = "Enter a valid email address.";
            return;
        }

        if (Password.Length < 6)
        {
            ErrorMessage = "Password must be at least 6 characters.";
            return;
        }

        // Firebase's email/password error messages are user-readable
        // (e.g. "The email address is already in use by another account").
        await RunAuthAsync(action, "Couldn't sign in. Please try again.", showExceptionMessage: true);
    }

    private async Task RunAuthAsync(Func<Task> action, string failureMessage, bool showExceptionMessage = false)
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
        catch (Exception ex)
        {
            Debug.WriteLine($"[Plated] Sign-in failed: {ex}");
#if DEBUG
            ErrorMessage = $"{failureMessage} ({ex.Message})";
#else
            ErrorMessage = showExceptionMessage ? ex.Message : failureMessage;
#endif
        }
        finally
        {
            IsBusy = false;
        }
    }
}
