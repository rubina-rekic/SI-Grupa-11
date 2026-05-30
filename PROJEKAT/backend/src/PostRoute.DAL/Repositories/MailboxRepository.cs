using Microsoft.EntityFrameworkCore;
using PostRoute.DAL.Entities;

namespace PostRoute.DAL.Repositories;

public class MailboxRepository : IMailboxRepository
{
    private readonly AppDbContext _context;

    public MailboxRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Mailbox?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Mailboxes
            .FirstOrDefaultAsync(m => m.Id == id && m.IsActive, cancellationToken);
    }

    public async Task<Mailbox?> GetBySerialNumberAsync(string serialNumber, CancellationToken cancellationToken)
    {
        return await _context.Mailboxes
            .FirstOrDefaultAsync(m => m.SerialNumber == serialNumber && m.IsActive, cancellationToken);
    }

    public async Task<IEnumerable<Mailbox>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Mailboxes
            .Where(m => m.IsActive)
            .OrderBy(m => m.SerialNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<Mailbox> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        MailboxType? type,
        MailboxPriority? priority,
        MailboxStatus? status,
        string? addressSearch,
        bool sortByPriority,
        CancellationToken cancellationToken)
    {
        var query = _context.Mailboxes
            .Where(m => m.IsActive)
            .AsQueryable();

        if (type.HasValue)
            query = query.Where(m => m.Type == type.Value);

        if (priority.HasValue)
            query = query.Where(m => m.Priority == priority.Value);

        if (status.HasValue)
            query = query.Where(m => m.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(addressSearch))
        {
            var needle = addressSearch.Trim().ToLower();
            query = query.Where(m =>
                m.Address.ToLower().Contains(needle) ||
                m.SerialNumber.ToLower().Contains(needle) ||
                m.Id.ToString().ToLower().Contains(needle));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        query = sortByPriority
            ? query.OrderBy(m => m.Priority).ThenBy(m => m.SerialNumber)
            : query.OrderBy(m => m.SerialNumber);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<bool> SerialNumberExistsAsync(string serialNumber, CancellationToken cancellationToken)
    {
        return await _context.Mailboxes
            .AnyAsync(m => m.SerialNumber == serialNumber && m.IsActive, cancellationToken);
    }

    public async Task<bool> SerialNumberExistsAsync(string serialNumber, Guid? excludeId, CancellationToken cancellationToken)
    {
        if (excludeId == null)
            return await SerialNumberExistsAsync(serialNumber, cancellationToken);

        return await _context.Mailboxes
            .AnyAsync(m => m.SerialNumber == serialNumber && m.Id != excludeId.Value && m.IsActive, cancellationToken);
    }

    public async Task<Mailbox> AddAsync(Mailbox mailbox, CancellationToken cancellationToken)
    {
        mailbox.CreatedAt = DateTime.UtcNow;
        mailbox.UpdatedAt = DateTime.UtcNow;

        _context.Mailboxes.Add(mailbox);
        await _context.SaveChangesAsync(cancellationToken);

        return mailbox;
    }

    public async Task<Mailbox> UpdateAsync(Mailbox mailbox, CancellationToken cancellationToken)
    {
        mailbox.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return mailbox;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var mailbox = await _context.Mailboxes
            .FirstOrDefaultAsync(m => m.Id == id && m.IsActive, cancellationToken);

        if (mailbox is null)
        {
            return false;
        }

        // Soft delete to preserve historical route references
        mailbox.IsActive = false;
        mailbox.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
