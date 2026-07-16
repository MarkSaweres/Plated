using Firebase.Auth;
using Google.SignIn;
using Plated.Core.Services;

namespace Plated.Services;

/// <summary>
/// Native iOS Google sign-in via the legacy Google Sign-In SDK (the same one
/// Plugin.Firebase.Auth.Google 3.1.2 wrapped internally), exchanged for a Firebase session.
/// <see cref="Google.SignIn.SignIn.SharedInstance"/>'s ClientId must be configured once at
/// launch (see AppDelegate.FinishedLaunching) and OpenUrl must forward to
/// <see cref="Google.SignIn.SignIn.HandleUrl"/> for the sign-in redirect to complete.
/// </summary>
public class GoogleSignInService : IGoogleSignInService
{
    public Task SignInAsync()
    {
        var tcs = new TaskCompletionSource();
        EventHandler<SignInDelegateEventArgs>? handler = null;

        handler = async (_, e) =>
        {
            SignIn.SharedInstance.SignedIn -= handler;

            if (e.Error is not null)
            {
                tcs.TrySetException(new InvalidOperationException(e.Error.LocalizedDescription));
                return;
            }

            try
            {
                var idToken = e.User.Authentication.IdToken
                    ?? throw new InvalidOperationException("Google sign-in did not return an ID token.");

                var credential = GoogleAuthProvider.GetCredential(idToken, e.User.Authentication.AccessToken);
                await Auth.DefaultInstance!.SignInWithCredentialAsync(credential);
                tcs.TrySetResult();
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        };

        SignIn.SharedInstance.SignedIn += handler;
        SignIn.SharedInstance.PresentingViewController = UIKit.UIApplication.SharedApplication.KeyWindow?.RootViewController;
        SignIn.SharedInstance.SignInUser();

        return tcs.Task;
    }
}
