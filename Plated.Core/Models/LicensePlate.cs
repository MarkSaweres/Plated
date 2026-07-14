using Plugin.Firebase.Firestore;

namespace Plated.Core.Models;

public class LicensePlate
{
    [FirestoreDocumentId]
    public string Id { get; set; } = string.Empty;

    [FirestoreProperty("plateNumber")]
    public string PlateNumber { get; set; } = string.Empty;

    [FirestoreProperty("state")]
    public string State { get; set; } = string.Empty;

    [FirestoreProperty("commentCount")]
    public int CommentCount { get; set; }

    [FirestoreProperty("createdAt")]
    public DateTimeOffset CreatedAt { get; set; }

    [FirestoreProperty("createdByUid")]
    public string CreatedByUid { get; set; } = string.Empty;

    public static string BuildId(string state, string plateNumber)
        => $"{Normalize(state)}-{Normalize(plateNumber)}";

    public static string Normalize(string value)
        => new string(value.Where(char.IsLetterOrDigit).ToArray()).ToUpperInvariant();
}
