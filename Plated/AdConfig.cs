namespace Plated;

/// <summary>
/// AdMob ad unit IDs. These are Google's public TEST IDs, safe to click while developing.
/// Before releasing, create your app and a banner ad unit in AdMob (admob.google.com) and replace
/// them, along with the app IDs in Platforms/Android/AndroidManifest.xml and Platforms/iOS/Info.plist.
/// </summary>
public static class AdConfig
{
#if ANDROID
    /// <summary>Must match the APPLICATION_ID meta-data in AndroidManifest.xml.</summary>
    public const string AppId = "ca-app-pub-3940256099942544~3347511713";

    public const string BannerAdUnitId = "ca-app-pub-3940256099942544/6300978111";
    public const string InterstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712";
#else
    public const string BannerAdUnitId = "ca-app-pub-3940256099942544/2934735716";
    public const string InterstitialAdUnitId = "ca-app-pub-3940256099942544/4411468910";
#endif
}
