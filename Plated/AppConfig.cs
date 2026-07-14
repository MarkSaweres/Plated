namespace Plated;

public static class AppConfig
{
    /// <summary>
    /// Firebase Console → Authentication → Sign-in method → Google → Web SDK configuration → Web client ID.
    /// Required on Android only; iOS reads its client ID from GoogleService-Info.plist automatically.
    /// </summary>
    public const string GoogleWebClientId = "YOUR_WEB_CLIENT_ID.apps.googleusercontent.com";
}
