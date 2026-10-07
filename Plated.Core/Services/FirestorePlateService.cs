using Plated.Core.Models;
using Plugin.Firebase.Firestore;

namespace Plated.Core.Services;

public class FirestorePlateService : IPlateService
{
    private const int HideAfterReportCount = 3;

    public async Task<LicensePlate?> FindPlateAsync(string state, string plateNumber)
    {
        var id = LicensePlate.BuildId(state, plateNumber);
        var snapshot = await CrossFirebaseFirestore.Current
            .GetDocument($"plates/{id}")
            .GetDocumentSnapshotAsync<LicensePlate>();

        return snapshot.Data;
    }

    public async Task<LicensePlate> GetOrCreatePlateAsync(string state, string plateNumber, string creatorUid)
    {
        var existing = await FindPlateAsync(state, plateNumber);
        if (existing is not null)
        {
            return existing;
        }

        var id = LicensePlate.BuildId(state, plateNumber);
        var normalizedState = LicensePlate.Normalize(state);
        var normalizedPlateNumber = LicensePlate.Normalize(plateNumber);

        await CrossFirebaseFirestore.Current
            .GetDocument($"plates/{id}")
            .SetDataAsync(new Dictionary<object, object>
            {
                ["plateNumber"] = normalizedPlateNumber,
                ["state"] = normalizedState,
                ["commentCount"] = 0,
                ["createdAt"] = FieldValue.ServerTimestamp(),
                ["createdByUid"] = creatorUid,
            });

        return new LicensePlate
        {
            Id = id,
            State = normalizedState,
            PlateNumber = normalizedPlateNumber,
            CommentCount = 0,
            CreatedAt = DateTimeOffset.UtcNow,
            CreatedByUid = creatorUid,
        };
    }

    public async Task<IReadOnlyList<PlateComment>> GetCommentsAsync(string plateId)
    {
        var snapshot = await CrossFirebaseFirestore.Current
            .GetCollection($"plates/{plateId}/comments")
            .OrderBy("createdAt", descending: true)
            .GetDocumentsAsync<PlateComment>();

        return snapshot.Documents
            .Select(d => d.Data)
            .Where(c => !c.IsHidden)
            .Select(c =>
            {
                c.PlateId = plateId;
                return c;
            })
            .ToList();
    }

    public async Task<PlateComment> AddCommentAsync(string plateId, AppUser author, string text, string? photoUrl)
    {
        // Don't use AddDocumentAsync(object): the plugin reads [FirestoreProperty] attributes off the
        // object's properties, so a Dictionary would be written as an empty document.
        var docRef = CrossFirebaseFirestore.Current
            .GetCollection($"plates/{plateId}/comments")
            .CreateDocument();

        await docRef.SetDataAsync(new Dictionary<object, object>
        {
            ["authorUid"] = author.Uid,
            ["authorDisplayName"] = author.DisplayName,
            ["authorPhotoUrl"] = author.PhotoUrl ?? string.Empty,
            ["text"] = text,
            ["photoUrl"] = photoUrl ?? string.Empty,
            ["createdAt"] = FieldValue.ServerTimestamp(),
            ["reportCount"] = 0,
            ["isHidden"] = false,
        });

        await CrossFirebaseFirestore.Current
            .GetDocument($"plates/{plateId}")
            .UpdateDataAsync(new Dictionary<object, object>
            {
                ["commentCount"] = FieldValue.IntegerIncrement(1),
            });

        return new PlateComment
        {
            Id = docRef.Id,
            PlateId = plateId,
            AuthorUid = author.Uid,
            AuthorDisplayName = author.DisplayName,
            AuthorPhotoUrl = author.PhotoUrl,
            Text = text,
            PhotoUrl = photoUrl,
            CreatedAt = DateTimeOffset.UtcNow,
            ReportCount = 0,
            IsHidden = false,
        };
    }

    public async Task ReportCommentAsync(string plateId, string commentId, string reporterUid, ReportReason reason, string? details)
    {
        await CrossFirebaseFirestore.Current
            .GetCollection($"plates/{plateId}/comments/{commentId}/reports")
            .CreateDocument()
            .SetDataAsync(new Dictionary<object, object>
            {
                ["reporterUid"] = reporterUid,
                ["reason"] = reason.ToString(),
                ["details"] = details ?? string.Empty,
                ["createdAt"] = FieldValue.ServerTimestamp(),
            });

        var commentRef = CrossFirebaseFirestore.Current.GetDocument($"plates/{plateId}/comments/{commentId}");
        var snapshot = await commentRef.GetDocumentSnapshotAsync<PlateComment>();
        var newReportCount = (snapshot.Data?.ReportCount ?? 0) + 1;

        var updates = new Dictionary<object, object>
        {
            ["reportCount"] = FieldValue.IntegerIncrement(1),
        };
        if (newReportCount >= HideAfterReportCount)
        {
            updates["isHidden"] = true;
        }

        await commentRef.UpdateDataAsync(updates);
    }
}
