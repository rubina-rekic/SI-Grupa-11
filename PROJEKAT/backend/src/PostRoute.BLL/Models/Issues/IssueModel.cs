using PostRoute.DAL.Entities;

namespace PostRoute.BLL.Models.Issues;

public sealed record IssueModel(
    Guid Id,
    Guid RouteItemId,
    Guid MailboxId,
    string MailboxAddress,
    string MailboxSerialNumber,
    Guid ReportedByUserId,
    string ReportedByUsername,
    string? UnavailableReason,
    IssueStatus Status,
    string StatusLabel,
    IssueAction? AssignedAction,
    string? AssignedActionLabel,
    Guid? ActionAssignedToUserId,
    string? ActionAssignedToUsername,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    IReadOnlyList<IssueCommentModel> Comments,
    IReadOnlyList<IssueTimelineEntryModel> Timeline
);

public sealed record IssueSummaryModel(
    Guid Id,
    Guid RouteItemId,
    Guid MailboxId,
    string MailboxAddress,
    string ReportedByUsername,
    string? UnavailableReason,
    IssueStatus Status,
    string StatusLabel,
    IssueAction? AssignedAction,
    DateTime CreatedAt
);

public sealed record IssueCommentModel(
    Guid Id,
    Guid AuthorId,
    string AuthorUsername,
    string AuthorRole,
    string Content,
    DateTime CreatedAt
);

public sealed record IssueTimelineEntryModel(
    string Type,        // "created" | "comment" | "action" | "status"
    string Description,
    string? ActorUsername,
    DateTime Timestamp
);

public sealed record IssueNotificationModel(
    Guid Id,
    Guid IssueId,
    string MailboxAddress,
    string Title,
    string Message,
    bool IsRead,
    DateTime CreatedAt
);