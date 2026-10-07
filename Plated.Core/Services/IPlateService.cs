using Plated.Core.Models;

namespace Plated.Core.Services;

public interface IPlateService
{
    Task<LicensePlate?> FindPlateAsync(string state, string plateNumber);

    Task<LicensePlate> GetOrCreatePlateAsync(string state, string plateNumber, string creatorUid);

    Task<IReadOnlyList<PlateComment>> GetCommentsAsync(string plateId);

    Task<PlateComment> AddCommentAsync(string plateId, AppUser author, string text);

    Task ReportCommentAsync(string plateId, string commentId, string reporterUid, ReportReason reason, string? details);
}
