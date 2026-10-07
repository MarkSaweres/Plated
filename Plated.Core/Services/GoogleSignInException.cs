namespace Plated.Core.Services;

/// <summary>A Google sign-in failure whose <see cref="Exception.Message"/> is safe to show to the user.</summary>
public class GoogleSignInException : Exception
{
    public GoogleSignInException(string message, string? details = null)
        : base(message)
    {
        Details = details;
    }

    /// <summary>Technical detail from the platform, for logs and Debug builds.</summary>
    public string? Details { get; }
}
