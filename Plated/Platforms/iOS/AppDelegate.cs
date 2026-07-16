using Foundation;
using Google.SignIn;
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

        var googleServicesInfo = NSMutableDictionary.FromFile("GoogleService-Info.plist");
        SignIn.SharedInstance.ClientId = googleServicesInfo["CLIENT_ID"]!.ToString()!;

        return base.FinishedLaunching(app, options);
    }

    public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        => SignIn.SharedInstance.HandleUrl(url) || base.OpenUrl(app, url, options);
}
