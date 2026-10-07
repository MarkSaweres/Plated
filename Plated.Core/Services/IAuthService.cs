using Plated.Core.Models;

namespace Plated.Core.Services;

public interface IAuthService
{
    AppUser? CurrentUser { get; }

    event EventHandler<AppUser?>? AuthStateChanged;

    Task<AppUser> SignInWithGoogleAsync();

    Task<AppUser> SignInWithEmailAsync(string email, string password);

    Task<AppUser> CreateAccountWithEmailAsync(string email, string password);

    Task SignOutAsync();
}
