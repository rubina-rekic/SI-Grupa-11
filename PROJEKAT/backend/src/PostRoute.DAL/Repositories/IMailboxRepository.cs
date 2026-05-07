using PostRoute.DAL.Entities;

namespace PostRoute.DAL.Repositories;

public interface IMailboxRepository
{
    Task<Mailbox?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Mailbox?> GetBySerialNumberAsync(string serialNumber, CancellationToken cancellationToken);
    Task<IEnumerable<Mailbox>> GetAllAsync(CancellationToken cancellationToken);
    Task<(IReadOnlyList<Mailbox> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        MailboxType? type,
        MailboxPriority? priority,
        string? addressSearch,
        bool sortByPriority,
        CancellationToken cancellationToken);
    Task<bool> SerialNumberExistsAsync(string serialNumber, CancellationToken cancellationToken);
    Task<bool> SerialNumberExistsAsync(string serialNumber, Guid? excludeId, CancellationToken cancellationToken);
    Task<Mailbox> AddAsync(Mailbox mailbox, CancellationToken cancellationToken);
    Task<Mailbox> UpdateAsync(Mailbox mailbox, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
