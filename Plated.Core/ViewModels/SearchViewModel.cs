using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Plated.Core.Models;
using Plated.Core.Navigation;
using Plated.Core.Services;

namespace Plated.Core.ViewModels;

public partial class SearchViewModel : BaseViewModel
{
    public IReadOnlyList<string> States => UsStates.Codes;

    [ObservableProperty]
    private string selectedState = "CA";

    [ObservableProperty]
    private string plateNumber = string.Empty;

    [ObservableProperty]
    private bool hasSearched;

    private readonly IInterstitialAdService _interstitialAds;

    public SearchViewModel(IInterstitialAdService interstitialAds)
    {
        _interstitialAds = interstitialAds;
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        if (string.IsNullOrWhiteSpace(PlateNumber) || IsBusy)
        {
            return;
        }

        IsBusy = true;
        ErrorMessage = null;
        try
        {
            HasSearched = true;

            // Full-screen ad first (no-op when ads are removed or none is ready), then the results.
            await _interstitialAds.ShowIfReadyAsync();

            await Shell.Current.GoToAsync(
                $"{Routes.PlateDetail}?{Routes.StateQueryKey}={SelectedState}&{Routes.PlateNumberQueryKey}={Uri.EscapeDataString(PlateNumber)}");
        }
        catch (Exception ex)
        {
            ShowError("Something went wrong. Please try again.", ex);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
