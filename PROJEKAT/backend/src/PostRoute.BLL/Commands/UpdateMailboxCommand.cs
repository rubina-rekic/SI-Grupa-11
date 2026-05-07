using PostRoute.DAL.Entities;

namespace PostRoute.BLL.Commands;

public sealed record UpdateMailboxCommand(
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
    Guid UserId
);
