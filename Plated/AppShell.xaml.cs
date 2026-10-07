using Plated.Core.Navigation;
using Plated.Core.UI;
using Plated.Views;

namespace Plated;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        PageStyling.ApplyLightStatusBar(this);
        Routing.RegisterRoute(Routes.PlateDetail, typeof(PlateDetailPage));
    }
}
