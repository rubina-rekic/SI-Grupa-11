using System.ComponentModel.DataAnnotations;

namespace PostRoute.DAL.Entities;

public class IssueComment
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid IssueId { get; set; }
    public Issue Issue { get; set; } = null!;

    [Required]
    public Guid AuthorId { get; set; }
    public User Author { get; set; } = null!;

    [Required]
    [MaxLength(1000)]
    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}