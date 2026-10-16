using Plated.Core.UI;
using Plated.Core.ViewModels;

namespace Plated.Views;

public partial class VerifyEmailPage : ContentPage
{
    private readonly VerifyEmailViewModel _viewModel;
    private IDispatcherTimer? _pollTimer;

    public VerifyEmailPage(VerifyEmailViewModel viewModel)
    {
        InitializeComponent();
        PageStyling.ApplyLightStatusBar(this);
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Move on automatically once the link has been tapped (typically in the browser or mail app).
        _pollTimer ??= Dispatcher.CreateTimer();
        _pollTimer.Interval = TimeSpan.FromSeconds(4);
        _pollTimer.Tick -= OnPollTick;
        _pollTimer.Tick += OnPollTick;
        _pollTimer.Start();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _pollTimer?.Stop();
    }

    private async void OnPollTick(object? sender, EventArgs e) => await _viewModel.PollAsync();
}
