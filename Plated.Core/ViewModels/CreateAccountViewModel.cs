using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plated.Core.Navigation;
using Plated.Core.Services;

namespace Plated.Core.ViewModels;

public partial class CreateAccountViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private readonly IAppNavigator _navigator;

    [ObservableProperty]
    private string firstName = string.Empty;

    [ObservableProperty]
    private string lastName = string.Empty;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    public CreateAccountViewModel(IAuthService authService, IAppNavigator navigator)
    {
        _authService = authService;
        _navigator = navigator;
    }

    [RelayCommand]
    private Task BackAsync() => _navigator.GoBackAsync();

    [RelayCommand]
    private async Task CreateAccountAsync()
    {
        if (IsBusy)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
        {
            ErrorMessage = "Enter your first and last name.";
            return;
        }

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

        IsBusy = true;
        ErrorMessage = null;
        try
        {
            await _authService.CreateAccountWithEmailAsync(FirstName, LastName, Email, Password);

            try
            {
                await _authService.SendEmailVerificationAsync();
            }
            catch (Exception ex)
            {
                // The verify screen has a Resend button, so don't fail account creation over this.
                System.Diagnostics.Debug.WriteLine($"[Plated] Sending verification email failed: {ex.Message}");
            }

            _navigator.ShowVerifyEmail();
        }
        catch (Exception ex)
        {
            ShowAuthError(ex, "Couldn't create your account. Please try again.");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
