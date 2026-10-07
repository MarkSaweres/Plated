using Plugin.Firebase.Firestore;

namespace Plated.Core.Models;

public class PlateComment
{
    [FirestoreDocumentId]
    public string Id { get; set; } = string.Empty;

    public string PlateId { get; set; } = string.Empty;

    [FirestoreProperty("authorUid")]
    public string AuthorUid { get; set; } = string.Empty;

    [FirestoreProperty("authorDisplayName")]
    public string AuthorDisplayName { get; set; } = string.Empty;

    [FirestoreProperty("authorPhotoUrl")]
    public string? AuthorPhotoUrl { get; set; }

    [FirestoreProperty("text")]
    public string Text { get; set; } = string.Empty;

    [FirestoreProperty("createdAt")]
    public DateTimeOffset CreatedAt { get; set; }

    [FirestoreProperty("reportCount")]
    public int ReportCount { get; set; }

    [FirestoreProperty("isHidden")]
    public bool IsHidden { get; set; }
}
