using Plugin.Firebase.Core.Exceptions;

namespace Plated.Core.Services;

/// <summary>Turns Firebase auth failures into short messages a person can act on.</summary>
public static class AuthErrorMessages
{
    /// <summary>Returns a friendly message, or null if the error isn't one we recognize.</summary>
    public static string? TryDescribe(Exception ex)
    {
        if (ex is CrossPlatformFirebaseAuthException authException)
        {
            // Firebase's email-enumeration protection reports a wrong password and an unknown
            // email identically, so InvalidCredentials covers "no such account" too.
            return FirebaseAuthErrorClassifier.TryClassify(authException) switch
            {
                FirebaseAuthFailure.UserNotFound => "We couldn't find an account with that email.",
                FirebaseAuthFailure.InvalidCredentials => "We couldn't find an account with that email and password.",
                FirebaseAuthFailure.UserCollision => "An account with that email already exists. Try signing in instead.",
                FirebaseAuthFailure.WeakPassword => "Choose a stronger password (at least 6 characters).",
                FirebaseAuthFailure.TooManyRequests => "Too many attempts. Please wait a moment and try again.",
                _ => null,
            };
        }

        if (ex.Message.Contains("network", StringComparison.OrdinalIgnoreCase))
        {
            return "Can't reach the server. Check your internet connection and try again.";
        }

        return null;
    }
}
