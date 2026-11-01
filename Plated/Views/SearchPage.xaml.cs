using Microsoft.Maui;
using Plated.Core.ViewModels;

namespace Plated.Views;

public partial class SearchPage : ContentPage
{
    private bool _hasAnimated;

    public SearchPage(SearchViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // The plate slides up into place the first time the screen appears.
        if (_hasAnimated)
        {
            return;
        }

        _hasAnimated = true;
        PlateCard.Opacity = 0;
        PlateCard.TranslationY = 28;
        await Task.WhenAll(
            PlateCard.FadeToAsync(1, 450, Easing.CubicOut),
            PlateCard.TranslateToAsync(0, 0, 450, Easing.CubicOut));
    }
}
