using Plated.Core.Navigation;
using Plated.Views;

namespace Plated;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(Routes.PlateDetail, typeof(PlateDetailPage));
    }
}
