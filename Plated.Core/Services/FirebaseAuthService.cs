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

        await CrossFirebaseFirestore.Current
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

        return user;
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
