using Foundation;
using Plugin.Firebase.Auth.Google;
using Plugin.Firebase.Core.Platforms.iOS;
using UIKit;

namespace Plated;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    public override bool FinishedLaunching(UIApplication app, NSDictionary options)
    {
        CrossFirebase.Initialize();
        FirebaseAuthGoogleImplementation.Initialize();
        return base.FinishedLaunching(app, options);
    }

    public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        => FirebaseAuthGoogleImplementation.OpenUrl(app, url, options) || base.OpenUrl(app, url, options);
}
