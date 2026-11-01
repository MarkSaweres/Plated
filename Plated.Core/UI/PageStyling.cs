using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Plated.Core.UI;

public static class PageStyling
{
    private static readonly Color PageBackground = Color.FromArgb("#F6F7FB");

    /// <summary>Matches the status bar to the light page background with dark icons and clock.</summary>
#pragma warning disable CA1416 // StatusBarBehavior is only unsupported on macOS/Windows, which this app doesn't target.
    public static void ApplyLightStatusBar(Page page)
    {
        page.Behaviors.Add(new StatusBarBehavior
        {
            StatusBarColor = PageBackground,
            StatusBarStyle = StatusBarStyle.DarkContent,
        });
    }

    private static readonly Color HeroNavy = Color.FromArgb("#0B1B33");

    /// <summary>Navy status bar with light icons, to sit seamlessly above the gradient page headers.</summary>
    public static void ApplyDarkStatusBar(Page page)
    {
        page.Behaviors.Add(new StatusBarBehavior
        {
            StatusBarColor = HeroNavy,
            StatusBarStyle = StatusBarStyle.LightContent,
        });
    }
#pragma warning restore CA1416
}
