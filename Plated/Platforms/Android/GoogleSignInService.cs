using Android.OS;
using AndroidX.Core.Content;
using AndroidX.Credentials;
using Firebase.Auth;
using Google.Android.Libraries.Identity.GoogleId;
using Microsoft.Maui.ApplicationModel;
using Plated.Core.Services;

namespace Plated.Services;

/// <summary>
/// Native Android Google sign-in via Credential Manager ("Sign in with Google"), exchanged for a
/// Firebase session with <c>FirebaseAuth.Instance.SignInWithCredentialAsync</c>.
/// </summary>
public class GoogleSignInService : IGoogleSignInService
{
    // Bundle key used by GoogleIdTokenCredential; the binding doesn't expose its createFrom helper.
    private const string IdTokenBundleKey = "com.google.android.libraries.identity.googleid.BUNDLE_KEY_ID_TOKEN";

    public async Task SignInAsync()
    {
        var activity = Platform.CurrentActivity
            ?? throw new GoogleSignInException("Couldn't start Google sign-in. Please try again.");

        var googleOption = new GetSignInWithGoogleOption.Builder(AppConfig.GoogleWebClientId).Build();
        var request = new GetCredentialRequest.Builder()
            .AddCredentialOption(googleOption)
            .Build();

        var callback = new GoogleCredentialCallback();
        CredentialManager.Create(activity).GetCredentialAsync(
            activity, request, null, ContextCompat.GetMainExecutor(activity)!, callback);

        var response = await callback.Result;
        var credential = response.Credential;

        var isGoogleCredential = credential.Type == GoogleIdTokenCredential.TypeGoogleIdTokenCredential
            || credential.Type == GoogleIdTokenCredential.TypeGoogleIdTokenSiwgCredential;
        if (!isGoogleCredential)
        {
            throw new GoogleSignInException(
                "Google sign-in returned an unexpected result. Please try again.", $"Credential type: {credential.Type}");
        }

        var idToken = ReadIdToken(credential.Data)
            ?? throw new GoogleSignInException("Google sign-in didn't return a token. Please try again.");

        var firebaseCredential = GoogleAuthProvider.GetCredential(idToken, null);
        await FirebaseAuth.Instance.SignInWithCredentialAsync(firebaseCredential);
    }

    private static string? ReadIdToken(Bundle data)
    {
        var token = data.GetString(IdTokenBundleKey);
        if (!string.IsNullOrEmpty(token))
        {
            return token;
        }

        // Fall back to scanning for the key in case the constant changes between library versions.
        foreach (var key in data.KeySet() ?? [])
        {
            if (key.EndsWith("ID_TOKEN", StringComparison.Ordinal))
            {
                token = data.GetString(key);
                if (!string.IsNullOrEmpty(token))
                {
                    return token;
                }
            }
        }

        return null;
    }
}

/// <summary>Bridges Credential Manager's Java callback to a Task.</summary>
internal sealed class GoogleCredentialCallback : Java.Lang.Object, ICredentialManagerCallback
{
    private readonly TaskCompletionSource<GetCredentialResponse> _completion = new();

    public Task<GetCredentialResponse> Result => _completion.Task;

    public void OnResult(Java.Lang.Object? result)
    {
        if (result is GetCredentialResponse response)
        {
            _completion.TrySetResult(response);
        }
        else
        {
            _completion.TrySetException(new GoogleSignInException(
                "Google sign-in returned an unexpected result. Please try again.", result?.ToString()));
        }
    }

    public void OnError(Java.Lang.Object e)
    {
        var className = e.Class.Name ?? string.Empty;
        var details = e.ToString();

        if (className.EndsWith("GetCredentialCancellationException", StringComparison.Ordinal))
        {
            _completion.TrySetCanceled();
        }
        else if (className.EndsWith("NoCredentialException", StringComparison.Ordinal))
        {
            _completion.TrySetException(new GoogleSignInException(
                "No Google account was found on this device. Add one in Settings, or sign in with email.", details));
        }
        else
        {
            _completion.TrySetException(new GoogleSignInException(
                "Google sign-in isn't available right now. Try signing in with email.", details));
        }
    }
}
