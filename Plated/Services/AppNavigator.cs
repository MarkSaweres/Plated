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

    public void ShowMainApp()
    {
        Application.Current!.Windows[0].Page = _serviceProvider.GetRequiredService<AppShell>();
    }

    public void ShowLogin()
    {
        Application.Current!.Windows[0].Page = _serviceProvider.GetRequiredService<LoginPage>();
    }
}
