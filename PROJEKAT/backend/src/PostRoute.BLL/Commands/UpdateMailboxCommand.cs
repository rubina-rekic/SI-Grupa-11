using PostRoute.DAL.Entities;

namespace PostRoute.BLL.Commands;

public record UpdateMailboxCommand(
    Guid Id,
    string SerialNumber,
    string Address,
    decimal Latitude,
    decimal Longitude,
    MailboxType Type,
    MailboxPriority Priority,
    int Capacity,
    int InstallationYear,
    string? Notes,
    Guid UserId,
    string? Reason,

    // US-32: Dostupnost
    bool IsAlwaysAvailable = true,
    TimeOnly? Slot1Start = null,
    TimeOnly? Slot1End = null,
    TimeOnly? Slot2Start = null,
    TimeOnly? Slot2End = null
);
