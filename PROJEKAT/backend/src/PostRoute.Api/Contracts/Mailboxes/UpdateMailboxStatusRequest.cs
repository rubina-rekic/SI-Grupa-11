using System.ComponentModel.DataAnnotations;
using PostRoute.DAL.Entities;

namespace PostRoute.Api.Contracts.Mailboxes;

public class UpdateMailboxStatusRequest
{
    [Required]
    public MailboxStatus Status { get; set; }

    [MaxLength(200)]
    public string? Reason { get; set; }
}
