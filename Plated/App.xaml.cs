using Microsoft.Extensions.DependencyInjection;
using Plated.Core.Services;
using Plated.Views;

namespace Plated;

public partial class App : Application
{
    private readonly IAuthService _authService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IPurchaseService _purchaseService;

    public App(IAuthService authService, IServiceProvider serviceProvider, IPurchaseService purchaseService)
    {
        InitializeComponent();

        // The design is light-only for now.
        UserAppTheme = Microsoft.Maui.ApplicationModel.AppTheme.Light;
        _authService = authService;
        _serviceProvider = serviceProvider;
        _purchaseService = purchaseService;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        Page rootPage = _authService.CurrentUser is not null
            ? _serviceProvider.GetRequiredService<AppShell>()
            : new NavigationPage(_serviceProvider.GetRequiredService<LoginPage>());

        var window = new Window(rootPage);

        // Pick up an existing "Remove ads" purchase (reinstall, new phone) once the activity is up.
        window.Created += (_, _) => _ = _purchaseService.RefreshAsync();
        return window;
    }
}
