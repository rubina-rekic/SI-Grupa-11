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
