using PostRoute.DAL.Entities;

namespace PostRoute.Api.Contracts.Mailboxes;

public record MailboxResponse(
    Guid Id,
    string SerialNumber,
    string Address,
    decimal Latitude,
    decimal Longitude,
    MailboxType Type,
    MailboxPriority Priority,
    MailboxStatus Status,
    int Capacity,
    int InstallationYear,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    string? Notes
);
