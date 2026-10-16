using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plated.Core.Navigation;
using Plated.Core.Services;

namespace Plated.Core.ViewModels;

public partial class VerifyEmailViewModel : BaseViewModel
{
    private const int ResendCooldownSeconds = 60;

    private readonly IAuthService _authService;
    private readonly IAppNavigator _navigator;

    [ObservableProperty]
    private string email;

    [ObservableProperty]
    private string? statusMessage;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanResend))]
    [NotifyPropertyChangedFor(nameof(ResendText))]
    private int resendSecondsLeft;

    public VerifyEmailViewModel(IAuthService authService, IAppNavigator navigator)
    {
        _authService = authService;
        _navigator = navigator;
        email = authService.CurrentUser?.Email ?? string.Empty;

        // A verification email was just sent on account creation; start the cooldown so people
        // don't immediately hammer Resend (Firebase rate-limits repeated sends).
        _ = StartCooldownAsync();
    }

    public bool CanResend => ResendSecondsLeft == 0;

    public string ResendText => CanResend ? "Resend email" : $"Resend in {ResendSecondsLeft}s";

    /// <summary>Quietly checks whether the link was tapped; used by the page's timer.</summary>
    public async Task PollAsync()
    {
        if (IsBusy)
        {
            return;
        }

        try
        {
            if (await _authService.RefreshEmailVerificationAsync())
            {
                _navigator.ShowMainApp();
            }
        }
        catch (Exception)
        {
            // Offline or transient; the next tick (or the button) will try again.
        }
    }

    [RelayCommand]
    private async Task CheckAsync()
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        ErrorMessage = null;
        StatusMessage = null;
        try
        {
            if (await _authService.RefreshEmailVerificationAsync())
            {
                _navigator.ShowMainApp();
            }
            else
            {
                StatusMessage = "Not verified yet. Open the link in the email we sent, then try again.";
            }
        }
        catch (Exception ex)
        {
            ShowError("Couldn't check right now. Check your connection and try again.", ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ResendAsync()
    {
        if (IsBusy || !CanResend)
        {
            return;
        }

        IsBusy = true;
        ErrorMessage = null;
        StatusMessage = null;
        try
        {
            await _authService.SendEmailVerificationAsync();
            StatusMessage = $"We sent a new link to {Email}.";
            _ = StartCooldownAsync();
        }
        catch (Exception ex)
        {
            ShowAuthError(ex, "Couldn't send the email. Please try again in a minute.");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SignOutAsync()
    {
        try
        {
            await _authService.SignOutAsync();
        }
        finally
        {
            _navigator.ShowLogin();
        }
    }

    private async Task StartCooldownAsync()
    {
        ResendSecondsLeft = ResendCooldownSeconds;
        while (ResendSecondsLeft > 0)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            ResendSecondsLeft--;
        }
    }
}
