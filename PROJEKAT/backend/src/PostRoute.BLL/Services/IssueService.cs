using PostRoute.BLL.Models.Issues;
using PostRoute.DAL.Entities;
using PostRoute.DAL.Repositories;

namespace PostRoute.BLL.Services;

public class IssueService : IIssueService
{
    private readonly IIssueRepository _issueRepository;

    public IssueService(IIssueRepository issueRepository)
    {
        _issueRepository = issueRepository;
    }

    public async Task<IssueModel> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var issue = await _issueRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Problem nije pronađen.");
        return MapToModel(issue);
    }

    public async Task<IEnumerable<IssueSummaryModel>> GetAllAsync(IssueStatus? status, CancellationToken cancellationToken)
    {
        var issues = await _issueRepository.GetAllAsync(status, cancellationToken);
        return issues.Select(MapToSummary);
    }

public async Task<IssueModel> AddCommentAsync(
    Guid issueId, Guid authorId, string authorRole, string content,
    CancellationToken cancellationToken)
{
    if (string.IsNullOrWhiteSpace(content))
        throw new InvalidOperationException("Komentar ne može biti prazan.");

    var issue = await _issueRepository.GetByIdAsync(issueId, cancellationToken)
        ?? throw new InvalidOperationException("Problem nije pronađen.");

    // EF Core sam prepoznaje vezu kroz kolekciju, nema potrebe za ručnim Id-em niti dodjelom 'comment.Issue = issue'
    var comment = new IssueComment
    {
        AuthorId = authorId,
        Content = content.Trim(),
        CreatedAt = DateTime.UtcNow
    };
    issue.Comments.Add(comment);

    // Automatski prebaci status u UObradi ako je bio Otvoren
    if (issue.Status == IssueStatus.Otvoren)
    {
        AddStatusHistory(issue, IssueStatus.Otvoren, IssueStatus.UObradi, authorId, "Dodan komentar");
        issue.Status = IssueStatus.UObradi;
    }

    issue.UpdatedAt = DateTime.UtcNow;

    // Kreiraj notifikaciju za drugu stranu
    var recipientId = authorRole == "PostalWorker"
        ? (Guid?)null
        : issue.ReportedByUserId;

    if (recipientId.HasValue)
    {
        issue.Notifications.Add(new IssueNotification
        {
            RecipientUserId = recipientId.Value,
            Title = $"Novi komentar — {ShortenAddress(issue.Mailbox.Address)}",
            Message = $"Dispečer je ostavio komentar na problem: {new string(content.Trim().Take(100).ToArray())}",
            CreatedAt = DateTime.UtcNow
        });
    }

    await _issueRepository.UpdateAsync(issue, cancellationToken);

    // ✅ Re-fetch da bi Author i sva navigacijska svojstva bila učitana
    var updated = await _issueRepository.GetByIdAsync(issueId, cancellationToken)
        ?? throw new InvalidOperationException("Problem nije pronađen.");
    return MapToModel(updated);
}

public async Task<IssueModel> AssignActionAsync(
    Guid issueId, Guid dispatcherId, IssueAction action, Guid? targetPostmanId,
    CancellationToken cancellationToken)
{
    var issue = await _issueRepository.GetByIdAsync(issueId, cancellationToken)
        ?? throw new InvalidOperationException("Problem nije pronađen.");

    if (issue.Status == IssueStatus.Rijesen)
        throw new InvalidOperationException("Ne može se mijenjati akcija riješenog problema.");

    if (action == IssueAction.DrugiPostar && targetPostmanId is null)
        throw new InvalidOperationException("Morate odabrati poštara za dodjelu.");

    issue.AssignedAction = action;
    issue.ActionAssignedByUserId = dispatcherId;
    issue.ActionAssignedToUserId = targetPostmanId;
    issue.ActionAssignedAt = DateTime.UtcNow;

    if (issue.Status == IssueStatus.Otvoren)
    {
        AddStatusHistory(issue, IssueStatus.Otvoren, IssueStatus.UObradi, dispatcherId, $"Dodijeljena akcija: {GetActionLabel(action)}");
        issue.Status = IssueStatus.UObradi;
    }

    issue.UpdatedAt = DateTime.UtcNow;

    // Notifikacija poštaru
    var notifRecipient = targetPostmanId ?? issue.ReportedByUserId;
    issue.Notifications.Add(new IssueNotification
    {
        RecipientUserId = notifRecipient,
        Title = $"Nova akcija — {ShortenAddress(issue.Mailbox.Address)}",
        Message = $"Dispečer je dodijelio akciju: {GetActionLabel(action)}",
        CreatedAt = DateTime.UtcNow
    });

    await _issueRepository.UpdateAsync(issue, cancellationToken);

    // ✅ Re-fetch da bi ActionAssignedToUser i sva navigacijska svojstva bila učitana
    var updated = await _issueRepository.GetByIdAsync(issueId, cancellationToken)
        ?? throw new InvalidOperationException("Problem nije pronađen.");
    return MapToModel(updated);
}

// Pomoćna metoda očišćena od eksplicitnih ID-eva i navigacijskih kaskada
private static void AddStatusHistory(Issue issue, IssueStatus oldStatus, IssueStatus newStatus, Guid userId, string note)
{
    issue.StatusHistory.Add(new IssueStatusHistory
    {
        ChangedByUserId = userId,
        OldStatus = oldStatus,
        NewStatus = newStatus,
        Note = note,
        ChangedAt = DateTime.UtcNow
    });
}



    public async Task<IssueModel> ResolveAsync(Guid issueId, Guid userId, CancellationToken cancellationToken)
    {
        var issue = await _issueRepository.GetByIdAsync(issueId, cancellationToken)
            ?? throw new InvalidOperationException("Problem nije pronađen.");

        if (issue.Status == IssueStatus.Rijesen)
            throw new InvalidOperationException("Problem je već označen kao riješen.");

        AddStatusHistory(issue, issue.Status, IssueStatus.Rijesen, userId, "Problem označen kao riješen");
        issue.Status = IssueStatus.Rijesen;
        issue.UpdatedAt = DateTime.UtcNow;

        await _issueRepository.UpdateAsync(issue, cancellationToken);

        // ✅ Re-fetch da bi ChangedByUser i sva navigacijska svojstva bila učitana
        var updated = await _issueRepository.GetByIdAsync(issueId, cancellationToken)
            ?? throw new InvalidOperationException("Problem nije pronađen.");
        return MapToModel(updated);
    }

    public async Task<IEnumerable<IssueNotificationModel>> GetMyNotificationsAsync(
        Guid userId, CancellationToken cancellationToken)
    {
        var notifications = await _issueRepository.GetTodayNotificationsAsync(userId, cancellationToken);
        return notifications.Select(n => new IssueNotificationModel(
            n.Id,
            n.IssueId,
            n.Issue.Mailbox.Address,
            n.Title,
            n.Message,
            n.IsRead,
            n.CreatedAt
        ));
    }

    public async Task MarkNotificationReadAsync(Guid notificationId, Guid userId, CancellationToken cancellationToken)
    {
        var notification = await _issueRepository.GetNotificationByIdAsync(notificationId, cancellationToken)
            ?? throw new InvalidOperationException("Notifikacija nije pronađena.");

        if (notification.RecipientUserId != userId)
            throw new InvalidOperationException("Nemate pristup ovoj notifikaciji.");

        notification.IsRead = true;
        await _issueRepository.UpdateNotificationAsync(notification, cancellationToken);
    }

    // ── private helpers ──────────────────────────────────────────



    private static string GetStatusLabel(IssueStatus status) => status switch
    {
        IssueStatus.Otvoren => "Otvoren",
        IssueStatus.UObradi => "U obradi",
        IssueStatus.Rijesen => "Riješen",
        _ => status.ToString()
    };

    private static string GetActionLabel(IssueAction action) => action switch
    {
        IssueAction.PonovniPokusaj => "Ponovni pokušaj",
        IssueAction.DrugiPostar => "Dodijeli drugom poštaru",
        IssueAction.OdgodaZasutra => "Ostavi za naredni dan",
        _ => action.ToString()
    };

    private static IssueModel MapToModel(Issue i)
    {
        var timeline = BuildTimeline(i);
        return new IssueModel(
            i.Id,
            i.RouteItemId,
            i.MailboxId,
            i.Mailbox.Address,
            i.Mailbox.SerialNumber,
            i.ReportedByUserId,
            i.ReportedBy.Username,
            i.UnavailableReason,
            i.Status,
            GetStatusLabel(i.Status),
            i.AssignedAction,
            i.AssignedAction.HasValue ? GetActionLabel(i.AssignedAction.Value) : null,
            i.ActionAssignedToUserId,
            i.ActionAssignedToUser?.Username,
            i.CreatedAt,
            i.UpdatedAt,
            i.Comments.OrderBy(c => c.CreatedAt).Select(c => new IssueCommentModel(
                c.Id, c.AuthorId, c.Author.Username, c.Author.Role, c.Content, c.CreatedAt
            )).ToList(),
            timeline
        );
    }

    private static IssueSummaryModel MapToSummary(Issue i) => new(
        i.Id,
        i.RouteItemId,
        i.MailboxId,
        i.Mailbox.Address,
        i.ReportedBy.Username,
        i.UnavailableReason,
        i.Status,
        GetStatusLabel(i.Status),
        i.AssignedAction,
        i.CreatedAt
    );

    private static IReadOnlyList<IssueTimelineEntryModel> BuildTimeline(Issue issue)
    {
        var entries = new List<(DateTime Timestamp, IssueTimelineEntryModel Entry)>();

        entries.Add((issue.CreatedAt, new IssueTimelineEntryModel(
            "created",
            $"Problem prijavljen — razlog: {issue.UnavailableReason ?? "nije naveden"}",
            issue.ReportedBy.Username,
            issue.CreatedAt
        )));

        foreach (var comment in issue.Comments.OrderBy(c => c.CreatedAt))
            entries.Add((comment.CreatedAt, new IssueTimelineEntryModel(
                "comment",
                $"Komentar: {comment.Content}",
                comment.Author.Username,
                comment.CreatedAt
            )));

        foreach (var history in issue.StatusHistory.OrderBy(h => h.ChangedAt))
            entries.Add((history.ChangedAt, new IssueTimelineEntryModel(
                "status",
                $"Status promijenjen: {GetStatusLabel(history.OldStatus)} → {GetStatusLabel(history.NewStatus)}{(history.Note != null ? $" ({history.Note})" : "")}",
                history.ChangedByUser?.Username,
                history.ChangedAt
            )));

        if (issue.AssignedAction.HasValue && issue.ActionAssignedAt.HasValue)
            entries.Add((issue.ActionAssignedAt.Value, new IssueTimelineEntryModel(
                "action",
                $"Dodijeljena akcija: {GetActionLabel(issue.AssignedAction.Value)}" +
                (issue.ActionAssignedToUser != null ? $" → {issue.ActionAssignedToUser.Username}" : ""),
                issue.ActionAssignedByUser?.Username,
                issue.ActionAssignedAt.Value
            )));

        return entries.OrderBy(e => e.Timestamp).Select(e => e.Entry).ToList();
    }

    // Dodaj private helper metodu:
private static string ShortenAddress(string address)
{
    // Uzmi samo prvi segment prije prvog zareza
    var firstPart = address.Split(',')[0].Trim();
    return firstPart.Length > 0 ? firstPart : address;
}
}