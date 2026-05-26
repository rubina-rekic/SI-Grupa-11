using Microsoft.EntityFrameworkCore;
using PostRoute.DAL.Entities;

namespace PostRoute.DAL.Repositories;

public class IssueRepository : IIssueRepository
{
    private readonly AppDbContext _context;

    public IssueRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Issue?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => await _context.Issues
            .Include(i => i.Mailbox)
            .Include(i => i.RouteItem)
                .ThenInclude(ri => ri.Route)
            .Include(i => i.ReportedBy)
            .Include(i => i.ActionAssignedToUser)
            .Include(i => i.ActionAssignedByUser)
            .Include(i => i.Comments)
                .ThenInclude(c => c.Author)
            .Include(i => i.StatusHistory)
                .ThenInclude(h => h.ChangedByUser)
            .Include(i => i.Notifications)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

    public async Task<Issue?> GetByRouteItemIdAsync(Guid routeItemId, CancellationToken cancellationToken)
        => await _context.Issues
            .FirstOrDefaultAsync(i => i.RouteItemId == routeItemId, cancellationToken);

    public async Task<IEnumerable<Issue>> GetAllAsync(IssueStatus? status, CancellationToken cancellationToken)
        => await _context.Issues
            .Include(i => i.Mailbox)
            .Include(i => i.ReportedBy)
            .Include(i => i.RouteItem)
                .ThenInclude(ri => ri.Route)
            .Where(i => status == null || i.Status == status)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Issue>> GetByPostmanIdAsync(Guid postmanId, CancellationToken cancellationToken)
        => await _context.Issues
            .Include(i => i.Mailbox)
            .Include(i => i.RouteItem)
                .ThenInclude(ri => ri.Route)
            .Where(i => i.ReportedByUserId == postmanId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<Issue> AddAsync(Issue issue, CancellationToken cancellationToken)
    {
        _context.Issues.Add(issue);
        await _context.SaveChangesAsync(cancellationToken);
        return issue;
    }

public async Task<Issue> UpdateAsync(Issue issue, CancellationToken cancellationToken)
{
    // Ako iz nekog razloga entitet dođe otkačen (npr. direktno sa API-ja), zakači ga
    if (_context.Entry(issue).State == EntityState.Detached)
    {
        _context.Issues.Update(issue);
    }

    // Pošto su objekti u kolekcijama sada sa praznim ID-em (Guid.Empty), 
    // EF Core će ih automatski prepoznati kao 'Added' i uraditi SQL INSERT.
    await _context.SaveChangesAsync(cancellationToken);
    return issue;
}

    public async Task<IEnumerable<IssueNotification>> GetUnreadNotificationsAsync(Guid userId, CancellationToken cancellationToken)
        => await _context.IssueNotifications
            .Include(n => n.Issue)
                .ThenInclude(i => i.Mailbox)
            .Where(n => n.RecipientUserId == userId && !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<IssueNotification>> GetTodayNotificationsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var todayUtc = DateTime.UtcNow.Date;
        return await _context.IssueNotifications
            .Include(n => n.Issue)
                .ThenInclude(i => i.Mailbox)
            .Where(n => n.RecipientUserId == userId && n.CreatedAt >= todayUtc)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IssueNotification?> GetNotificationByIdAsync(Guid id, CancellationToken cancellationToken)
        => await _context.IssueNotifications
            .FirstOrDefaultAsync(n => n.Id == id, cancellationToken);

    public async Task<IssueNotification> UpdateNotificationAsync(IssueNotification notification, CancellationToken cancellationToken)
    {
        _context.IssueNotifications.Update(notification);
        await _context.SaveChangesAsync(cancellationToken);
        return notification;
    }
}