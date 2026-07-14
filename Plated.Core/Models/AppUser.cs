using Plugin.Firebase.Firestore;

namespace Plated.Core.Models;

public class AppUser
{
    [FirestoreDocumentId]
    public string Uid { get; set; } = string.Empty;

    [FirestoreProperty("displayName")]
    public string DisplayName { get; set; } = string.Empty;

    [FirestoreProperty("email")]
    public string Email { get; set; } = string.Empty;

    [FirestoreProperty("photoUrl")]
    public string? PhotoUrl { get; set; }
}
