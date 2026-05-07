using System.ComponentModel.DataAnnotations;

namespace PostRoute.DAL.Entities;

public class MailboxAuditLog
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid MailboxId { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(50)]
    public string FieldName { get; set; } = string.Empty;

    [Required]
    public string? OldValue { get; set; }

    [Required]
    public string? NewValue { get; set; }

    [Required]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [Required]
    [MaxLength(20)]
    public string Action { get; set; } = string.Empty; // "UPDATE", "CREATE", "DELETE"

    // Navigation properties
    public Mailbox Mailbox { get; set; } = null!;
    public User User { get; set; } = null!;
}
