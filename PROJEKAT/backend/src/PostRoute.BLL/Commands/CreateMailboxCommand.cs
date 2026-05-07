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
    string? Notes = null
);
