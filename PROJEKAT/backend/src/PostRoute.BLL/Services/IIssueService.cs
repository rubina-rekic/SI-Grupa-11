using PostRoute.BLL.Models.Issues;
using PostRoute.DAL.Entities;

namespace PostRoute.BLL.Services;

public interface IIssueService
{
    Task<IssueModel> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<IssueSummaryModel>> GetAllAsync(IssueStatus? status, CancellationToken cancellationToken);
    Task<IssueModel> AddCommentAsync(Guid issueId, Guid authorId, string authorRole, string content, CancellationToken cancellationToken);
    Task<IssueModel> AssignActionAsync(Guid issueId, Guid dispatcherId, IssueAction action, Guid? targetPostmanId, CancellationToken cancellationToken);
    Task<IssueModel> ResolveAsync(Guid issueId, Guid userId, CancellationToken cancellationToken);
    Task<IEnumerable<IssueNotificationModel>> GetMyNotificationsAsync(Guid userId, CancellationToken cancellationToken);
    Task MarkNotificationReadAsync(Guid notificationId, Guid userId, CancellationToken cancellationToken);
}