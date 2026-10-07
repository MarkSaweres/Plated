using Android.App;
using Android.Content.PM;
using Android.OS;
using Microsoft.Maui.ApplicationModel;
using Plugin.Firebase.Core.Platforms.Android;

namespace Plated;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        CrossFirebase.Initialize(this, () => Platform.CurrentActivity!);
    }
}
