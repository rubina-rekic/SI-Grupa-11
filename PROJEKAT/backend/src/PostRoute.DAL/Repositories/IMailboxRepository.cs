using PostRoute.DAL.Entities;

namespace PostRoute.DAL.Repositories;

public interface IMailboxRepository
{
    Task<Mailbox?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Mailbox?> GetBySerialNumberAsync(string serialNumber, CancellationToken cancellationToken);
    Task<IEnumerable<Mailbox>> GetAllAsync(CancellationToken cancellationToken);
    Task<bool> SerialNumberExistsAsync(string serialNumber, CancellationToken cancellationToken);
    Task<Mailbox> AddAsync(Mailbox mailbox, CancellationToken cancellationToken);
    Task<Mailbox> UpdateAsync(Mailbox mailbox, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
