namespace Plated.Core.Services;

/// <summary>
/// Performs a native Google sign-in and exchanges the resulting token for a signed-in
/// Firebase session (via <c>FirebaseAuth.Instance.SignInWithCredentialAsync</c> on the
/// platform side). After this completes, the ambient Plugin.Firebase auth state
/// (<c>CrossFirebaseAuth.Current.CurrentUser</c> / auth-state listeners) reflects the
/// signed-in user, since it wraps the same native Firebase Auth singleton.
/// </summary>
public interface IGoogleSignInService
{
    Task SignInAsync();
}
