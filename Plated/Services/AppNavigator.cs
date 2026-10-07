using Microsoft.Extensions.DependencyInjection;
using Plated.Core.Navigation;
using Plated.Views;

namespace Plated.Services;

public class AppNavigator : IAppNavigator
{
    private readonly IServiceProvider _serviceProvider;

    public AppNavigator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    private static Page RootPage => Application.Current!.Windows[0].Page!;

    public void ShowMainApp()
    {
        Application.Current!.Windows[0].Page = _serviceProvider.GetRequiredService<AppShell>();
    }

    public void ShowLogin()
    {
        Application.Current!.Windows[0].Page = new NavigationPage(_serviceProvider.GetRequiredService<LoginPage>());
    }

    public Task ShowCreateAccountAsync()
        => RootPage.Navigation.PushAsync(_serviceProvider.GetRequiredService<CreateAccountPage>());

    public Task GoBackAsync()
        => RootPage.Navigation.PopAsync();
}
