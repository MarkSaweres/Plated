using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Plated.Core.Models;
using Plated.Core.Navigation;
using Plated.Core.Services;
using Plated.Core.UI;

namespace Plated.Core.ViewModels;

public partial class SearchViewModel : BaseViewModel
{
    private readonly IInterstitialAdService _interstitialAds;
    private readonly IRecentPlatesService _recentPlates;
    private readonly IPhotoCaptureService _photoCapture;
    private readonly IPlateOcrService _ocr;

    public SearchViewModel(
        IInterstitialAdService interstitialAds,
        IRecentPlatesService recentPlates,
        IPhotoCaptureService photoCapture,
        IPlateOcrService ocr)
    {
        _interstitialAds = interstitialAds;
        _recentPlates = recentPlates;
        _photoCapture = photoCapture;
        _ocr = ocr;

        foreach (var plate in _recentPlates.Load())
        {
            RecentPlates.Add(plate);
        }

        selectedStateName = UsStates.NameOf(selectedState);
    }

    /// <summary>Full names for the state picker (the plate card shows "California", not "CA").</summary>
    public IReadOnlyList<string> StateNames { get; } = UsStates.AllNames;

    public ObservableCollection<RecentPlate> RecentPlates { get; } = new();

    public bool HasRecent => RecentPlates.Count > 0;

    [ObservableProperty]
    private string selectedState = "CA";

    [ObservableProperty]
    private string selectedStateName;

    [ObservableProperty]
    private string plateNumber = string.Empty;

    [ObservableProperty]
    private bool hasSearched;

    [ObservableProperty]
    private bool isScanning;

    partial void OnSelectedStateNameChanged(string value)
        => SelectedState = UsStates.CodeOf(value);

    partial void OnSelectedStateChanged(string value)
        => SelectedStateName = UsStates.NameOf(value);

    [RelayCommand]
    private async Task SearchAsync()
    {
        if (string.IsNullOrWhiteSpace(PlateNumber) || IsBusy)
        {
            return;
        }

        Haptics.Tap();
        IsBusy = true;
        ErrorMessage = null;
        try
        {
            HasSearched = true;

            // Full-screen ad first (no-op when ads are removed or none is ready), then the results.
            await _interstitialAds.ShowIfReadyAsync();

            var number = PlateNumber.Trim();
            RememberPlate(SelectedState, number);

            await Shell.Current.GoToAsync(
                $"{Routes.PlateDetail}?{Routes.StateQueryKey}={SelectedState}&{Routes.PlateNumberQueryKey}={Uri.EscapeDataString(number)}");
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

    [RelayCommand]
    private async Task OpenRecentAsync(RecentPlate? plate)
    {
        if (plate is null)
        {
            return;
        }

        SelectedState = plate.State;
        PlateNumber = plate.Number;
        await SearchAsync();
    }

    [RelayCommand]
    private Task ScanWithCameraAsync() => ScanAsync(_photoCapture.CapturePhotoAsync);

    [RelayCommand]
    private Task ScanFromGalleryAsync() => ScanAsync(_photoCapture.PickPhotoAsync);

    private async Task ScanAsync(Func<Task<string?>> getPhoto)
    {
        if (IsBusy || IsScanning)
        {
            return;
        }

        Haptics.Tap();
        ErrorMessage = null;
        try
        {
            var path = await getPhoto();
            if (path is null)
            {
                return;
            }

            IsScanning = true;
            var recognized = await _ocr.RecognizePlateNumberAsync(path);
            if (string.IsNullOrEmpty(recognized))
            {
                ErrorMessage = "We couldn't read a plate in that photo. Try a closer, straighter shot, or type it in.";
            }
            else
            {
                PlateNumber = recognized;
            }
        }
        catch (Exception ex)
        {
            ShowError("We couldn't scan that photo. You can type the plate number instead.", ex);
        }
        finally
        {
            IsScanning = false;
        }
    }

    private void RememberPlate(string state, string number)
    {
        var updated = _recentPlates.Add(state, number);
        RecentPlates.Clear();
        foreach (var plate in updated)
        {
            RecentPlates.Add(plate);
        }

        OnPropertyChanged(nameof(HasRecent));
    }
}
