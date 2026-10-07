using System.Diagnostics;
using Plated.Core.Models;
using Plugin.Firebase.Auth;
using Plugin.Firebase.Firestore;

namespace Plated.Core.Services;

public class FirebaseAuthService : IAuthService
{
    private readonly IGoogleSignInService _googleSignInService;

    public AppUser? CurrentUser { get; private set; }

    public event EventHandler<AppUser?>? AuthStateChanged;

    public FirebaseAuthService(IGoogleSignInService googleSignInService)
    {
        _googleSignInService = googleSignInService;
        CurrentUser = ToAppUser(CrossFirebaseAuth.Current.CurrentUser);
        CrossFirebaseAuth.Current.AddAuthStateListener(auth =>
        {
            CurrentUser = ToAppUser(auth.CurrentUser);
            AuthStateChanged?.Invoke(this, CurrentUser);
        });
    }

    public async Task<AppUser> SignInWithGoogleAsync()
    {
        // Native sign-in exchanges credentials directly against FirebaseAuth.Instance on
        // each platform; CrossFirebaseAuth.Current wraps that same singleton, so CurrentUser
        // reflects the new session immediately once this completes.
        await _googleSignInService.SignInAsync();

        var user = ToAppUser(CrossFirebaseAuth.Current.CurrentUser)
            ?? throw new InvalidOperationException("Google sign-in completed but no Firebase user is signed in.");

        await SaveUserProfileAsync(user);
        return user;
    }

    public async Task<AppUser> SignInWithEmailAsync(string email, string password)
    {
        var firebaseUser = await CrossFirebaseAuth.Current.SignInWithEmailAndPasswordAsync(
            email.Trim(), password, createsUserAutomatically: false);

        var user = ToAppUser(firebaseUser)!;
        await SaveUserProfileAsync(user);
        return user;
    }

    public async Task<AppUser> CreateAccountWithEmailAsync(string email, string password)
    {
        var trimmedEmail = email.Trim();
        var firebaseUser = await CrossFirebaseAuth.Current.CreateUserAsync(trimmedEmail, password);

        // Email accounts have no display name; default to the part before the @.
        await firebaseUser.UpdateProfileAsync(displayName: trimmedEmail.Split('@')[0]);
        await CrossFirebaseAuth.Current.ReloadCurrentUserAsync();

        var user = ToAppUser(CrossFirebaseAuth.Current.CurrentUser)!;
        await SaveUserProfileAsync(user);
        return user;
    }

    /// <summary>
    /// Best-effort: the profile document isn't needed to be signed in, and a Firestore write
    /// waits indefinitely when the database isn't reachable/created, so never let it block login.
    /// </summary>
    private static async Task SaveUserProfileAsync(AppUser user)
    {
        try
        {
            var write = CrossFirebaseFirestore.Current
                .GetCollection("users")
                .GetDocument(user.Uid)
                .SetDataAsync(new Dictionary<object, object>
                {
                    ["uid"] = user.Uid,
                    ["displayName"] = user.DisplayName,
                    ["email"] = user.Email,
                    ["photoUrl"] = user.PhotoUrl ?? string.Empty,
                    ["lastSignInAt"] = FieldValue.ServerTimestamp(),
                }, SetOptions.Merge());

            var finished = await Task.WhenAny(write, Task.Delay(TimeSpan.FromSeconds(8)));
            if (finished == write)
            {
                await write;
            }
            else
            {
                Debug.WriteLine("[Plated] Saving user profile to Firestore timed out; continuing. " +
                                "Check that the Firestore database exists and rules are published.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[Plated] Saving user profile failed: {ex}");
        }
    }

    public Task SignOutAsync()
    {
        return CrossFirebaseAuth.Current.SignOutAsync();
    }

    private static AppUser? ToAppUser(IFirebaseUser? firebaseUser)
    {
        if (firebaseUser is null)
        {
            return null;
        }

        return new AppUser
        {
            Uid = firebaseUser.Uid,
            DisplayName = firebaseUser.DisplayName ?? "Anonymous",
            Email = firebaseUser.Email ?? string.Empty,
            PhotoUrl = firebaseUser.PhotoUrl,
        };
    }
}
