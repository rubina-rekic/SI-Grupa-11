using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PostRoute.DAL.Entities;

public class Issue
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid RouteItemId { get; set; }
    public RouteItem RouteItem { get; set; } = null!;

    [Required]
    public Guid MailboxId { get; set; }
    public Mailbox Mailbox { get; set; } = null!;

    [Required]
    public Guid ReportedByUserId { get; set; }
    public User ReportedBy { get; set; } = null!;

    [MaxLength(500)]
    public string? UnavailableReason { get; set; }

    public IssueStatus Status { get; set; } = IssueStatus.Otvoren;

    public IssueAction? AssignedAction { get; set; }

    public Guid? ActionAssignedToUserId { get; set; }
    public User? ActionAssignedToUser { get; set; }

    public Guid? ActionAssignedByUserId { get; set; }
    public User? ActionAssignedByUser { get; set; }

    public DateTime? ActionAssignedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<IssueComment> Comments { get; set; } = new List<IssueComment>();
    public ICollection<IssueStatusHistory> StatusHistory { get; set; } = new List<IssueStatusHistory>();
    public ICollection<IssueNotification> Notifications { get; set; } = new List<IssueNotification>();
}

public enum IssueStatus
{
    Otvoren = 0,
    UObradi = 1,
    Rijesen = 2
}

public enum IssueAction
{
    PonovniPokusaj = 0,
    DrugiPostar = 1,
    OdgodaZasutra = 2
}