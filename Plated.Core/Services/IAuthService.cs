using Plated.Core.Models;

namespace Plated.Core.Services;

public interface IAuthService
{
    AppUser? CurrentUser { get; }

    event EventHandler<AppUser?>? AuthStateChanged;

    Task<AppUser> SignInWithGoogleAsync();

    Task<AppUser> SignInWithEmailAsync(string email, string password);

    Task<AppUser> CreateAccountWithEmailAsync(string firstName, string lastName, string email, string password);

    /// <summary>Emails a verification link to the signed-in user.</summary>
    Task SendEmailVerificationAsync();

    /// <summary>
    /// Re-reads the signed-in user from Firebase and refreshes their token so security rules see the
    /// new verified status. Returns whether the email is now verified.
    /// </summary>
    Task<bool> RefreshEmailVerificationAsync();

    Task SignOutAsync();
}
