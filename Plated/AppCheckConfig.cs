using Plugin.Firebase.AppCheck;

namespace Plated;

/// <summary>
/// Firebase App Check proves a request comes from the genuine Plated app, so a script that only
/// has your public Firebase keys can't read or write your data once enforcement is switched on
/// in the Firebase console.
/// </summary>
public static class AppCheckConfig
{
#if DEBUG
    // Emulators and sideloaded debug builds can't pass the real checks. The debug provider logs a
    // one-time token you add under Firebase console > App Check > Apps > Manage debug tokens.
    public static AppCheckOptions Options => AppCheckOptions.Debug;
#elif ANDROID
    public static AppCheckOptions Options => AppCheckOptions.PlayIntegrity;
#else
    public static AppCheckOptions Options => AppCheckOptions.AppAttest;
#endif
}
