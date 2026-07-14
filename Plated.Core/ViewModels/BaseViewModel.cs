using CommunityToolkit.Mvvm.ComponentModel;

namespace Plated.Core.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string? errorMessage;
}
