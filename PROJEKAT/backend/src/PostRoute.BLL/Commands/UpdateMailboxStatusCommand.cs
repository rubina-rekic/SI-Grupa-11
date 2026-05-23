using PostRoute.DAL.Entities;

namespace PostRoute.BLL.Commands;

public record UpdateMailboxStatusCommand(
    Guid MailboxId,
    MailboxStatus NewStatus,
    Guid UserId,
    string? Reason = null
);
