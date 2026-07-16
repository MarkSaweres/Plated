using Android.App;
using Android.Content;
using Android.Gms.Auth.Api.Identity;
using Android.Gms.Extensions;
using Firebase.Auth;
using Microsoft.Maui.ApplicationModel;
using Plated.Core.Services;

namespace Plated.Services;

/// <summary>
/// Native Android Google sign-in via Play Services Identity (One Tap), exchanged for a
/// Firebase session. Google marks <see cref="ISignInClient.BeginSignIn"/> obsolete in favor
/// of Credential Manager, but it remains functional and is still the simplest reliable path
/// that doesn't require the newer androidx.credentials stack.
/// </summary>
public class GoogleSignInService : IGoogleSignInService
{
    private const int RequestCode = 9002;

    private TaskCompletionSource<Intent?>? _pendingSignIn;

#pragma warning disable CS0618 // BeginSignInRequest and its members are deprecated by Google in favor of Credential Manager; still functional.
    public async Task SignInAsync()
    {
        var activity = Platform.CurrentActivity
            ?? throw new InvalidOperationException("No current Android activity.");

        var signInClient = Identity.GetSignInClient(activity);

        var idTokenOptions = BeginSignInRequest.GoogleIdTokenRequestOptions.InvokeBuilder()
            .SetSupported(true)
            .SetServerClientId(AppConfig.GoogleWebClientId)
            .SetFilterByAuthorizedAccounts(false)
            .Build();

        var request = BeginSignInRequest.InvokeBuilder()
            .SetGoogleIdTokenRequestOptions(idTokenOptions)
            .Build();

        var beginSignInResult = await signInClient.BeginSignIn(request).AsAsync<BeginSignInResult>();

        _pendingSignIn = new TaskCompletionSource<Intent?>();
        activity.StartIntentSenderForResult(beginSignInResult.PendingIntent.IntentSender, RequestCode, null, 0, 0, 0);
        var data = await _pendingSignIn.Task;

        var credential = signInClient.GetSignInCredentialFromIntent(data);

        var idToken = credential.GoogleIdToken
            ?? throw new InvalidOperationException("Google sign-in did not return an ID token.");

        var firebaseCredential = GoogleAuthProvider.GetCredential(idToken, null);
        await FirebaseAuth.Instance.SignInWithCredentialAsync(firebaseCredential);
    }
#pragma warning restore CS0618

    public void HandleActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        if (requestCode == RequestCode)
        {
            _pendingSignIn?.TrySetResult(data);
        }
    }
}
