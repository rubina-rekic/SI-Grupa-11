using System.ComponentModel.DataAnnotations;

namespace PostRoute.DAL.Entities;

public class RouteItem
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid RouteId { get; set; }
    public Route Route { get; set; } = null!;

    [Required]
    public Guid MailboxId { get; set; }
    public Mailbox Mailbox { get; set; } = null!;

    [Required]
    public int Order { get; set; }

    public TimeOnly EstimatedArrivalTime { get; set; }

    public string Status { get; set; } = "Planirano";

    public bool IsManuallyReordered { get; set; } = false;
}
