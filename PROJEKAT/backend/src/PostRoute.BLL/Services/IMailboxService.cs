using PostRoute.BLL.Commands;
using PostRoute.BLL.Models;
using PostRoute.DAL.Entities;

namespace PostRoute.BLL.Services;

public interface IMailboxService
{
    Task<Mailbox> CreateAsync(CreateMailboxCommand command, CancellationToken cancellationToken);
    Task<Mailbox?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Mailbox?> GetBySerialNumberAsync(string serialNumber, CancellationToken cancellationToken);
    Task<IEnumerable<Mailbox>> GetAllAsync(CancellationToken cancellationToken);
    Task<PagedResult<Mailbox>> GetPagedAsync(
        int page,
        int pageSize,
        MailboxType? type,
        MailboxPriority? priority,
        string? addressSearch,
        bool sortByPriority,
        CancellationToken cancellationToken);
    Task<bool> SerialNumberExistsAsync(string serialNumber, CancellationToken cancellationToken);
}
