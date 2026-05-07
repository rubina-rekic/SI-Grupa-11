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
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<Mailbox?> GetBySerialNumberAsync(string serialNumber, CancellationToken cancellationToken)
    {
        return await _context.Mailboxes
            .FirstOrDefaultAsync(m => m.SerialNumber == serialNumber, cancellationToken);
    }

    public async Task<IEnumerable<Mailbox>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Mailboxes
            .OrderBy(m => m.SerialNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<Mailbox> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        MailboxType? type,
        MailboxPriority? priority,
        string? addressSearch,
        bool sortByPriority,
        CancellationToken cancellationToken)
    {
        var query = _context.Mailboxes.AsQueryable();

        if (type.HasValue)
            query = query.Where(m => m.Type == type.Value);

        if (priority.HasValue)
            query = query.Where(m => m.Priority == priority.Value);

        if (!string.IsNullOrWhiteSpace(addressSearch))
        {
            var needle = addressSearch.Trim().ToLower();
            query = query.Where(m => m.Address.ToLower().Contains(needle));
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
            .AnyAsync(m => m.SerialNumber == serialNumber, cancellationToken);
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

        _context.Mailboxes.Update(mailbox);
        await _context.SaveChangesAsync(cancellationToken);

        return mailbox;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var mailbox = await GetByIdAsync(id, cancellationToken);
        if (mailbox != null)
        {
            _context.Mailboxes.Remove(mailbox);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
