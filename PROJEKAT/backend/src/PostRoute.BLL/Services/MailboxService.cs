
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
        if (await _mailboxRepository.SerialNumberExistsAsync(command.SerialNumber, cancellationToken))
            throw new InvalidOperationException($"Sandučić sa serijskim brojem '{command.SerialNumber}' već postoji.");

        if (command.Latitude < -90 || command.Latitude > 90)
            throw new InvalidOperationException("Latitude mora biti između -90 i 90.");

        if (command.Longitude < -180 || command.Longitude > 180)
            throw new InvalidOperationException("Longitude mora biti između -180 i 180.");

        if (command.Capacity <= 0)
            throw new InvalidOperationException("Kapacitet mora biti veći od 0.");

        var currentYear = DateTime.Now.Year;
        if (command.InstallationYear < 1900 || command.InstallationYear > currentYear + 10)
            throw new InvalidOperationException($"Godina instalacije mora biti između 1900 i {currentYear + 10}.");

        // US-32: Validacija vremenskih okvira
        ValidateAvailability(
            command.IsAlwaysAvailable,
            command.Slot1Start, command.Slot1End,
            command.Slot2Start, command.Slot2End);

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
            Notes = command.Notes?.Trim(),

            // US-32
            IsAlwaysAvailable = command.IsAlwaysAvailable,
            Slot1Start = command.IsAlwaysAvailable ? null : command.Slot1Start,
            Slot1End = command.IsAlwaysAvailable ? null : command.Slot1End,
            Slot2Start = command.IsAlwaysAvailable ? null : command.Slot2Start,
            Slot2End = command.IsAlwaysAvailable ? null : command.Slot2End,
        };

        return await _mailboxRepository.AddAsync(mailbox, cancellationToken);
    }

    public async Task<Mailbox> UpdateAsync(UpdateMailboxCommand command, CancellationToken cancellationToken)
    {
        var existingMailbox = await _mailboxRepository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new InvalidOperationException("Sandučić nije pronađen.");

        if (command.Latitude < -90 || command.Latitude > 90)
            throw new InvalidOperationException("Latitude mora biti između -90 i 90.");

        if (command.Longitude < -180 || command.Longitude > 180)
            throw new InvalidOperationException("Longitude mora biti između -180 i 180.");

        if (command.Capacity <= 0)
            throw new InvalidOperationException("Kapacitet mora biti veći od 0.");

        var currentYear = DateTime.Now.Year;
        if (command.InstallationYear < 1900 || command.InstallationYear > currentYear + 10)
            throw new InvalidOperationException($"Godina instalacije mora biti između 1900 i {currentYear + 10}.");

        // US-32: Validacija vremenskih okvira
        ValidateAvailability(
            command.IsAlwaysAvailable,
            command.Slot1Start, command.Slot1End,
            command.Slot2Start, command.Slot2End);

        await LogChangesAsync(existingMailbox, command, cancellationToken);

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

        // US-32
        existingMailbox.IsAlwaysAvailable = command.IsAlwaysAvailable;
        existingMailbox.Slot1Start = command.IsAlwaysAvailable ? null : command.Slot1Start;
        existingMailbox.Slot1End = command.IsAlwaysAvailable ? null : command.Slot1End;
        existingMailbox.Slot2Start = command.IsAlwaysAvailable ? null : command.Slot2Start;
        existingMailbox.Slot2End = command.IsAlwaysAvailable ? null : command.Slot2End;

        return await _mailboxRepository.UpdateAsync(existingMailbox, cancellationToken);
    }

    // ---------------------------------------------------------------
    // US-32: Centralna validaciona logika za vremenske okvire
    // ---------------------------------------------------------------
    private static void ValidateAvailability(
        bool isAlwaysAvailable,
        TimeOnly? slot1Start, TimeOnly? slot1End,
        TimeOnly? slot2Start, TimeOnly? slot2End)
    {
        // Ako je 24/7, preskočiti sve provjere
        if (isAlwaysAvailable) return;

        if (!slot1Start.HasValue && !slot1End.HasValue)
            throw new InvalidOperationException("Ako sanducic nije 24/7 dostupan, morate definisati barem jedan vremenski period.");

        // Ako je uneseno samo jedno polje prvog termina
        if (slot1Start.HasValue != slot1End.HasValue)
            throw new InvalidOperationException("Morate unijeti i početak i kraj prvog termina.");

        // Validacija prvog termina: kraj mora biti nakon početka
        if (slot1Start.HasValue && slot1End.HasValue && slot1End <= slot1Start)
            throw new InvalidOperationException("Krajnje vrijeme mora biti nakon početnog.");

        // Validacija drugog termina — samo ako je uopće unesen
        bool hasSlot2 = slot2Start.HasValue || slot2End.HasValue;
        if (hasSlot2)
        {
            if (!slot1Start.HasValue)
                throw new InvalidOperationException("Drugi termin zahtijeva da je prvi termin definisan.");

            if (slot2Start.HasValue != slot2End.HasValue)
                throw new InvalidOperationException("Morate unijeti i početak i kraj drugog termina.");

            if (slot2Start.HasValue && slot2End.HasValue && slot2End <= slot2Start)
                throw new InvalidOperationException("Krajnje vrijeme drugog termina mora biti nakon početnog.");

            // Preklapanje termina: Slot2 ne smije početi prije nego Slot1 završi
            if (slot1End.HasValue && slot2Start.HasValue && slot2Start < slot1End)
                throw new InvalidOperationException("Drugi termin se preklapa s prvim. Unesi termini ne smiju imati preklapanje.");
        }
    }

    private async Task LogChangesAsync(Mailbox existingMailbox, UpdateMailboxCommand command, CancellationToken cancellationToken)
    {
        var changes = new List<(string FieldName, object? OldValue, object? NewValue)>
        {
            ("SerialNumber",     existingMailbox.SerialNumber,        command.SerialNumber),
            ("Address",          existingMailbox.Address,             command.Address),
            ("Latitude",         existingMailbox.Latitude,            command.Latitude),
            ("Longitude",        existingMailbox.Longitude,           command.Longitude),
            ("Type",             existingMailbox.Type,                command.Type),
            ("Priority",         existingMailbox.Priority,            command.Priority),
            ("Capacity",         existingMailbox.Capacity,            command.Capacity),
            ("InstallationYear", existingMailbox.InstallationYear,    command.InstallationYear),
            ("Notes",            existingMailbox.Notes,               command.Notes),
            // US-32
            ("IsAlwaysAvailable", existingMailbox.IsAlwaysAvailable,  command.IsAlwaysAvailable),
            ("Slot1Start",       existingMailbox.Slot1Start,          command.Slot1Start),
            ("Slot1End",         existingMailbox.Slot1End,            command.Slot1End),
            ("Slot2Start",       existingMailbox.Slot2Start,          command.Slot2Start),
            ("Slot2End",         existingMailbox.Slot2End,            command.Slot2End),
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
                    Action = "UPDATE",
                    Reason = change.FieldName == "Priority" ? command.Reason : null
                };

                await _auditLogRepository.LogAsync(auditLog, cancellationToken);
            }
        }
    }

    public async Task<Mailbox?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => await _mailboxRepository.GetByIdAsync(id, cancellationToken);

    public async Task<Mailbox?> GetBySerialNumberAsync(string serialNumber, CancellationToken cancellationToken)
        => await _mailboxRepository.GetBySerialNumberAsync(serialNumber, cancellationToken);

    public async Task<IEnumerable<Mailbox>> GetAllAsync(CancellationToken cancellationToken)
        => await _mailboxRepository.GetAllAsync(cancellationToken);

    public async Task<PagedResult<Mailbox>> GetPagedAsync(
        int page, int pageSize,
        MailboxType? type, MailboxPriority? priority,
        string? addressSearch, bool sortByPriority,
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
        => await _mailboxRepository.SerialNumberExistsAsync(serialNumber, cancellationToken);

    public async Task<bool> SerialNumberExistsAsync(string serialNumber, Guid? excludeId, CancellationToken cancellationToken)
        => await _mailboxRepository.SerialNumberExistsAsync(serialNumber, excludeId, cancellationToken);

    public async Task<IEnumerable<MailboxAuditLog>> GetAuditLogAsync(Guid mailboxId, CancellationToken cancellationToken)
        => await _auditLogRepository.GetByMailboxIdAsync(mailboxId, cancellationToken);

    public async Task DeleteAsync(Guid mailboxId, CancellationToken cancellationToken)
    {
        var deleted = await _mailboxRepository.DeleteAsync(mailboxId, cancellationToken);
        if (!deleted)
        {
            throw new InvalidOperationException("Sanducic nije pronadjen.");
        }
    }
}
