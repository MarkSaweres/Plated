namespace Plated.Core.Models;

public enum ReportReason
{
    Spam,
    Harassment,
    Inaccurate,
    Other,
}

public class CommentReport
{
    public string Id { get; set; } = string.Empty;

    public string PlateId { get; set; } = string.Empty;

    public string CommentId { get; set; } = string.Empty;

    public string ReporterUid { get; set; } = string.Empty;

    public ReportReason Reason { get; set; }

    public string? Details { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
}
