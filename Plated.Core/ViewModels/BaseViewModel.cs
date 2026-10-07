using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using Plated.Core.Services;

namespace Plated.Core.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string? errorMessage;

    /// <summary>
    /// Logs the exception and shows a friendly message; Debug builds append the real reason
    /// so problems (rules, network, config) are diagnosable from the device screen.
    /// </summary>
    protected void ShowError(string friendlyMessage, Exception ex)
    {
        Debug.WriteLine($"[Plated] {friendlyMessage} {ex}");
#if DEBUG
        ErrorMessage = $"{friendlyMessage} ({ex.Message})";
#else
        ErrorMessage = friendlyMessage;
#endif
    }

    /// <summary>Friendly message for known auth failures; falls back to <see cref="ShowError"/>.</summary>
    protected void ShowAuthError(Exception ex, string fallbackMessage)
    {
        var friendly = AuthErrorMessages.TryDescribe(ex);
        if (friendly is null)
        {
            ShowError(fallbackMessage, ex);
            return;
        }

        Debug.WriteLine($"[Plated] Auth failed: {ex}");
        ErrorMessage = friendly;
    }
}
