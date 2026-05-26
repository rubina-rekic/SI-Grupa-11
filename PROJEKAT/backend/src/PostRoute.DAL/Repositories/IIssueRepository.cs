using PostRoute.DAL.Entities;

namespace PostRoute.DAL.Repositories;

public interface IIssueRepository
{
    Task<Issue?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Issue?> GetByRouteItemIdAsync(Guid routeItemId, CancellationToken cancellationToken);
    Task<IEnumerable<Issue>> GetAllAsync(IssueStatus? status, CancellationToken cancellationToken);
    Task<IEnumerable<Issue>> GetByPostmanIdAsync(Guid postmanId, CancellationToken cancellationToken);
    Task<Issue> AddAsync(Issue issue, CancellationToken cancellationToken);
    Task<Issue> UpdateAsync(Issue issue, CancellationToken cancellationToken);
    Task<IEnumerable<IssueNotification>> GetUnreadNotificationsAsync(Guid userId, CancellationToken cancellationToken);
    Task<IEnumerable<IssueNotification>> GetTodayNotificationsAsync(Guid userId, CancellationToken cancellationToken);
    Task<IssueNotification?> GetNotificationByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IssueNotification> UpdateNotificationAsync(IssueNotification notification, CancellationToken cancellationToken);
}