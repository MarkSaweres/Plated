using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Plated.Core.Models;
using Plated.Core.Navigation;
using Plated.Core.Services;

namespace Plated.Core.ViewModels;

public partial class AddEntryViewModel : BaseViewModel
{
    private readonly IPhotoCaptureService _photoCaptureService;
    private readonly IPlateOcrService _ocrService;
    private readonly IPlateService _plateService;
    private readonly IAuthService _authService;

    public IReadOnlyList<string> States => UsStates.Codes;

    [ObservableProperty]
    private string selectedState = "CA";

    [ObservableProperty]
    private string plateNumber = string.Empty;

    [ObservableProperty]
    private string? photoPath;

    [ObservableProperty]
    private string commentText = string.Empty;

    [ObservableProperty]
    private bool isScanning;

    public AddEntryViewModel(
        IPhotoCaptureService photoCaptureService,
        IPlateOcrService ocrService,
        IPlateService plateService,
        IAuthService authService)
    {
        _photoCaptureService = photoCaptureService;
        _ocrService = ocrService;
        _plateService = plateService;
        _authService = authService;
    }

    [RelayCommand]
    private async Task TakePhotoAsync()
    {
        var path = await _photoCaptureService.CapturePhotoAsync();
        if (path is null)
        {
            return;
        }

        PhotoPath = path;
        await RunOcrAsync(path);
    }

    [RelayCommand]
    private async Task PickPhotoAsync()
    {
        var path = await _photoCaptureService.PickPhotoAsync();
        if (path is null)
        {
            return;
        }

        PhotoPath = path;
        await RunOcrAsync(path);
    }

    private async Task RunOcrAsync(string path)
    {
        IsScanning = true;
        try
        {
            var recognized = await _ocrService.RecognizePlateNumberAsync(path);
            if (!string.IsNullOrEmpty(recognized))
            {
                PlateNumber = recognized;
            }
        }
        catch (Exception)
        {
            // Best-effort: the user can still type the plate manually.
        }
        finally
        {
            IsScanning = false;
        }
    }

    [RelayCommand]
    private async Task SubmitAsync()
    {
        if (string.IsNullOrWhiteSpace(PlateNumber) || string.IsNullOrWhiteSpace(CommentText) || IsBusy)
        {
            return;
        }

        var user = _authService.CurrentUser;
        if (user is null)
        {
            ErrorMessage = "Please sign in again.";
            return;
        }

        IsBusy = true;
        ErrorMessage = null;
        try
        {
            var plate = await _plateService.GetOrCreatePlateAsync(SelectedState, PlateNumber, user.Uid);

            await _plateService.AddCommentAsync(plate.Id, user, CommentText.Trim());

            var state = plate.State;
            var number = plate.PlateNumber;
            PlateNumber = string.Empty;
            CommentText = string.Empty;
            PhotoPath = null;

            await Shell.Current.GoToAsync(
                $"{Routes.PlateDetail}?{Routes.StateQueryKey}={state}&{Routes.PlateNumberQueryKey}={Uri.EscapeDataString(number)}");
        }
        catch (Exception ex)
        {
            ShowError("Couldn't submit your entry. Please try again.", ex);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
