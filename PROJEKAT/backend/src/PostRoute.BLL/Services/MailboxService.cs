using PostRoute.BLL.Commands;
using PostRoute.BLL.Models;
using PostRoute.DAL.Entities;
using PostRoute.DAL.Repositories;

namespace PostRoute.BLL.Services;

public class MailboxService : IMailboxService
{
    private readonly IMailboxRepository _mailboxRepository;
    private readonly IMailboxAuditLogRepository _auditLogRepository;

    public MailboxService(IMailboxRepository mailboxRepository, IMailboxAuditLogRepository auditLogRepository)
    {
        _mailboxRepository = mailboxRepository;
        _auditLogRepository = auditLogRepository;
    }

    public async Task<Mailbox> CreateAsync(CreateMailboxCommand command, CancellationToken cancellationToken)
    {
        // Validate unique serial number
        if (await _mailboxRepository.SerialNumberExistsAsync(command.SerialNumber, cancellationToken))
        {
            throw new InvalidOperationException($"Sandučić sa serijskim brojem '{command.SerialNumber}' već postoji.");
        }

        // Validate coordinates
        if (command.Latitude < -90 || command.Latitude > 90)
        {
            throw new InvalidOperationException("Latitude mora biti između -90 i 90.");
        }

        if (command.Longitude < -180 || command.Longitude > 180)
        {
            throw new InvalidOperationException("Longitude mora biti između -180 i 180.");
        }

        // Validate capacity
        if (command.Capacity <= 0)
        {
            throw new InvalidOperationException("Kapacitet mora biti veći od 0.");
        }

        // Validate installation year
        var currentYear = DateTime.Now.Year;
        if (command.InstallationYear < 1900 || command.InstallationYear > currentYear + 10)
        {
            throw new InvalidOperationException($"Godina instalacije mora biti između 1900 i {currentYear + 10}.");
        }

        var mailbox = new Mailbox
        {
            Id = Guid.NewGuid(),
            SerialNumber = command.SerialNumber.Trim(),
            Address = command.Address.Trim(),
            Latitude = command.Latitude,
            Longitude = command.Longitude,
            Type = command.Type,
            Priority = command.Priority,
            Capacity = command.Capacity,
            InstallationYear = command.InstallationYear,
            Notes = command.Notes?.Trim()
        };

        return await _mailboxRepository.AddAsync(mailbox, cancellationToken);
    }

    public async Task<Mailbox?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _mailboxRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Mailbox?> GetBySerialNumberAsync(string serialNumber, CancellationToken cancellationToken)
    {
        return await _mailboxRepository.GetBySerialNumberAsync(serialNumber, cancellationToken);
    }

    public async Task<IEnumerable<Mailbox>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _mailboxRepository.GetAllAsync(cancellationToken);
    }

    public async Task<PagedResult<Mailbox>> GetPagedAsync(
        int page,
        int pageSize,
        MailboxType? type,
        MailboxPriority? priority,
        string? addressSearch,
        bool sortByPriority,
        CancellationToken cancellationToken)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 25;
        if (pageSize > 100) pageSize = 100;

        var (items, total) = await _mailboxRepository.GetPagedAsync(
            page, pageSize, type, priority, addressSearch, sortByPriority, cancellationToken);

        return new PagedResult<Mailbox>(items, total, page, pageSize);
    }

    public async Task<bool> SerialNumberExistsAsync(string serialNumber, CancellationToken cancellationToken)
    {
        return await _mailboxRepository.SerialNumberExistsAsync(serialNumber, cancellationToken);
    }

    public async Task<bool> SerialNumberExistsAsync(string serialNumber, Guid? excludeId, CancellationToken cancellationToken)
    {
        return await _mailboxRepository.SerialNumberExistsAsync(serialNumber, excludeId, cancellationToken);
    }

    public async Task<Mailbox> UpdateAsync(UpdateMailboxCommand command, CancellationToken cancellationToken)
    {
        var existingMailbox = await _mailboxRepository.GetByIdAsync(command.Id, cancellationToken);
        if (existingMailbox == null)
        {
            throw new InvalidOperationException("Sandučić nije pronađen.");
        }

        // Skip serial number validation in edit mode - serial number remains the same

        // Validate coordinates
        if (command.Latitude < -90 || command.Latitude > 90)
        {
            throw new InvalidOperationException("Latitude mora biti između -90 i 90.");
        }

        if (command.Longitude < -180 || command.Longitude > 180)
        {
            throw new InvalidOperationException("Longitude mora biti između -180 i 180.");
        }

        // Validate capacity
        if (command.Capacity <= 0)
        {
            throw new InvalidOperationException("Kapacitet mora biti veći od 0.");
        }

        // Validate installation year
        var currentYear = DateTime.Now.Year;
        if (command.InstallationYear < 1900 || command.InstallationYear > currentYear + 10)
        {
            throw new InvalidOperationException($"Godina instalacije mora biti između 1900 i {currentYear + 10}.");
        }

        // Log changes
        await LogChangesAsync(existingMailbox, command, cancellationToken);

        // Update mailbox
        existingMailbox.SerialNumber = command.SerialNumber.Trim();
        existingMailbox.Address = command.Address.Trim();
        existingMailbox.Latitude = command.Latitude;
        existingMailbox.Longitude = command.Longitude;
        existingMailbox.Type = command.Type;
        existingMailbox.Priority = command.Priority;
        existingMailbox.Capacity = command.Capacity;
        existingMailbox.InstallationYear = command.InstallationYear;
        existingMailbox.Notes = command.Notes?.Trim();
        existingMailbox.UpdatedAt = DateTime.UtcNow;

        return await _mailboxRepository.UpdateAsync(existingMailbox, cancellationToken);
    }

    private async Task LogChangesAsync(Mailbox existingMailbox, UpdateMailboxCommand command, CancellationToken cancellationToken)
    {
        var changes = new List<(string FieldName, object? OldValue, object? NewValue)>
        {
            ("SerialNumber", existingMailbox.SerialNumber, command.SerialNumber),
            ("Address", existingMailbox.Address, command.Address),
            ("Latitude", existingMailbox.Latitude, command.Latitude),
            ("Longitude", existingMailbox.Longitude, command.Longitude),
            ("Type", existingMailbox.Type, command.Type),
            ("Priority", existingMailbox.Priority, command.Priority),
            ("Capacity", existingMailbox.Capacity, command.Capacity),
            ("InstallationYear", existingMailbox.InstallationYear, command.InstallationYear),
            ("Notes", existingMailbox.Notes, command.Notes)
        };

        foreach (var change in changes)
        {
            var oldValueStr = change.OldValue?.ToString();
            var newValueStr = change.NewValue?.ToString();

            if (!string.Equals(oldValueStr, newValueStr, StringComparison.Ordinal))
            {
                var auditLog = new MailboxAuditLog
                {
                    Id = Guid.NewGuid(),
                    MailboxId = command.Id,
                    UserId = command.UserId,
                    FieldName = change.FieldName,
                    OldValue = oldValueStr,
                    NewValue = newValueStr,
                    Action = "UPDATE"
                };

                await _auditLogRepository.LogAsync(auditLog, cancellationToken);
            }
        }
    }
}
