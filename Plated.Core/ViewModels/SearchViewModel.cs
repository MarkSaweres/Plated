using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Plated.Core.Models;
using Plated.Core.Navigation;

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
