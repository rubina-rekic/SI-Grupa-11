using PostRoute.DAL.Entities;

namespace PostRoute.BLL.Commands;

public record CreateMailboxCommand(
    string SerialNumber,
    string Address,
    decimal Latitude,
    decimal Longitude,
    MailboxType Type,
    int Capacity,
    int InstallationYear,
    string? Notes = null,
    MailboxPriority Priority = MailboxPriority.Srednji,
    string? Reason = null,

    // US-32: Dostupnost
    bool IsAlwaysAvailable = false,
    TimeOnly? Slot1Start = null,
    TimeOnly? Slot1End = null,
    TimeOnly? Slot2Start = null,
    TimeOnly? Slot2End = null
);