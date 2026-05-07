using PostRoute.DAL.Entities;

namespace PostRoute.DAL.Repositories;

public interface IMailboxAuditLogRepository
{
    Task LogAsync(MailboxAuditLog auditLog, CancellationToken cancellationToken);
    Task<IEnumerable<MailboxAuditLog>> GetByMailboxIdAsync(Guid mailboxId, CancellationToken cancellationToken);
    Task<IEnumerable<MailboxAuditLog>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
}
