namespace PostRoute.BLL.Models.Routes;

public class RouteItemResponse
{
    public Guid Id { get; set; }
    public Guid MailboxId { get; set; }
    public string Address { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public int Order { get; set; }
    public TimeOnly EstimatedArrivalTime { get; set; }
    public string Priority { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsManuallyReordered { get; set; }
    public string MailboxStatus { get; set; } = string.Empty;
    public DateTime? ProcessedAt { get; set; }
    public Guid? ProcessedBy { get; set; }
    public string? ProcessedStatus { get; set; }
    public string? UnavailableReason { get; set; }
}
