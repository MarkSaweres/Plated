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
    private readonly IPurchaseService _purchaseService;

    [ObservableProperty]
    private AppUser? currentUser;

    [ObservableProperty]
    private string? removeAdsPrice;

    [ObservableProperty]
    private string? purchaseMessage;

    public ProfileViewModel(IAuthService authService, IAppNavigator navigator, IPurchaseService purchaseService)
    {
        _authService = authService;
        _navigator = navigator;
        _purchaseService = purchaseService;
        CurrentUser = _authService.CurrentUser;
        _authService.AuthStateChanged += (_, user) => CurrentUser = user;
        _ = LoadPriceAsync();
    }

    /// <summary>Bound by the page to show either the upgrade card or the thank-you card.</summary>
    public AdsState AdsState => _purchaseService.AdsState;

    private async Task LoadPriceAsync()
    {
        RemoveAdsPrice = await _purchaseService.GetPriceAsync();
    }

    [RelayCommand]
    private async Task RemoveAdsAsync()
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        PurchaseMessage = null;
        try
        {
            PurchaseMessage = await _purchaseService.PurchaseRemoveAdsAsync() switch
            {
                PurchaseOutcome.Purchased => "Thank you! Ads are now removed.",
                PurchaseOutcome.Pending => "Your purchase is pending. Ads will go away once the payment clears.",
                PurchaseOutcome.Failed => "Couldn't complete the purchase. Please try again.",
                _ => null,
            };
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task RestorePurchasesAsync()
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        PurchaseMessage = null;
        try
        {
            PurchaseMessage = await _purchaseService.RestorePurchasesAsync()
                ? "Purchase restored. Ads are removed."
                : "We couldn't find a previous purchase on this account.";
        }
        finally
        {
            IsBusy = false;
        }
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
        catch (Exception ex)
        {
            ShowError("Couldn't sign out. Please try again.", ex);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
