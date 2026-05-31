namespace PostRoute.BLL.Models.Routes;

public class PostmanPerformanceReportResponse
{
    public DateOnly FromDate { get; set; }
    public DateOnly ToDate { get; set; }
    public int TotalPostmen { get; set; }
    public int TotalAssignedMailboxes { get; set; }
    public int TotalEmptiedLocations { get; set; }
    public int TotalUnrealizedLocations { get; set; }
    public decimal TeamAverageSuccessPercentage { get; set; }
    public List<PostmanPerformanceRowResponse> Rows { get; set; } = new();
}

public class PostmanPerformanceRowResponse
{
    public Guid PostmanId { get; set; }
    public string PostmanName { get; set; } = string.Empty;
    public int AssignedMailboxes { get; set; }
    public int EmptiedLocations { get; set; }
    public int UnrealizedLocations { get; set; }
    public decimal SuccessPercentage { get; set; }
    public int CompletedRoutesCount { get; set; }
    public List<PostmanPerformanceRouteResponse> Routes { get; set; } = new();
}

public class PostmanPerformanceRouteResponse
{
    public Guid RouteId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly PlannedStartTime { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int AssignedMailboxes { get; set; }
    public int EmptiedLocations { get; set; }
    public int UnrealizedLocations { get; set; }
    public decimal SuccessPercentage { get; set; }
}
