using Microsoft.EntityFrameworkCore;
using PostRoute.DAL.Entities;

namespace PostRoute.DAL.Repositories;

public class MailboxAuditLogRepository : IMailboxAuditLogRepository
{
    private readonly AppDbContext _context;

    public MailboxAuditLogRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task LogAsync(MailboxAuditLog auditLog, CancellationToken cancellationToken)
    {
        await _context.MailboxAuditLogs.AddAsync(auditLog, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<MailboxAuditLog>> GetByMailboxIdAsync(Guid mailboxId, CancellationToken cancellationToken)
    {
        return await _context.MailboxAuditLogs
            .Where(log => log.MailboxId == mailboxId)
            .Include(log => log.User)
            .OrderByDescending(log => log.Timestamp)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MailboxAuditLog>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _context.MailboxAuditLogs
            .Where(log => log.UserId == userId)
            .Include(log => log.Mailbox)
            .OrderByDescending(log => log.Timestamp)
            .ToListAsync(cancellationToken);
    }
}
