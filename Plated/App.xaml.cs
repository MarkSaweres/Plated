using Microsoft.Extensions.DependencyInjection;
using Plated.Core.Services;
using Plated.Views;

namespace Plated;

public partial class App : Application
{
    private readonly IAuthService _authService;
    private readonly IServiceProvider _serviceProvider;

    public App(IAuthService authService, IServiceProvider serviceProvider)
    {
        InitializeComponent();

        // The design is light-only for now.
        UserAppTheme = Microsoft.Maui.ApplicationModel.AppTheme.Light;
        _authService = authService;
        _serviceProvider = serviceProvider;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        Page rootPage = _authService.CurrentUser is not null
            ? _serviceProvider.GetRequiredService<AppShell>()
            : _serviceProvider.GetRequiredService<LoginPage>();

        return new Window(rootPage);
    }
}
