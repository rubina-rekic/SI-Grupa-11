using System.ComponentModel.DataAnnotations;

namespace PostRoute.DAL.Entities;

public class IssueNotification
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid IssueId { get; set; }
    public Issue Issue { get; set; } = null!;

    [Required]
    public Guid RecipientUserId { get; set; }
    public User Recipient { get; set; } = null!;

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Message { get; set; } = string.Empty;

    public bool IsRead { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}