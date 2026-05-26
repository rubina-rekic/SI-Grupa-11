using System.ComponentModel.DataAnnotations;

namespace PostRoute.DAL.Entities;

public class IssueStatusHistory
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid IssueId { get; set; }
    public Issue Issue { get; set; } = null!;

    public Guid? ChangedByUserId { get; set; }
    public User? ChangedByUser { get; set; }

    public IssueStatus OldStatus { get; set; }
    public IssueStatus NewStatus { get; set; }

    [MaxLength(200)]
    public string? Note { get; set; }

    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
}