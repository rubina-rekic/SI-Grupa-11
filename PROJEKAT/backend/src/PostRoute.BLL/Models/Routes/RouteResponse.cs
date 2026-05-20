namespace PostRoute.BLL.Models.Routes;

public class RouteResponse
{
    public Guid Id { get; set; }
    public Guid PostmanId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly PlannedStartTime { get; set; }
    public TimeOnly? PlannedEndTime { get; set; }
    public decimal TotalDistanceKm { get; set; }
    public int TotalDurationMinutes { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool ExceedsStandardTime { get; set; }
    public List<RouteItemResponse> RouteItems { get; set; } = new();
    
    public DateTime? LastReorderedAt { get; set; }
    public string? LastReorderedBy { get; set; }

    // Debug info for transparency
    public int TotalMailboxesCount { get; set; }
    public int ActiveMailboxesCount { get; set; }
    public int DayFilteredMailboxesCount { get; set; }
}
