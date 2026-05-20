using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PostRoute.DAL.Entities;

public class Route
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid PostmanId { get; set; }
    public User Postman { get; set; } = null!;

    [Required]
    public DateOnly Date { get; set; }

    [Required]
    public TimeOnly PlannedStartTime { get; set; }

    public TimeOnly? PlannedEndTime { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal TotalDistanceKm { get; set; }

    public int TotalDurationMinutes { get; set; }

    public RouteStatus Status { get; set; } = RouteStatus.Planirana;

    public bool ExceedsStandardTime { get; set; }

    public ICollection<RouteItem> RouteItems { get; set; } = new List<RouteItem>();
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastReorderedAt { get; set; }
    public string? LastReorderedBy { get; set; }

    public DateTime? AssignedAt { get; set; }
    public string? AssignedBy { get; set; }
}

public enum RouteStatus
{
    Planirana = 0,
    UProgresu = 1,
    Zavrsena = 2,
    Otkazana = 3,
    Dodijeljena = 4
}
