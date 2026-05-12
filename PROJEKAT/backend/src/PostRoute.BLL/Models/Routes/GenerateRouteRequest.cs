namespace PostRoute.BLL.Models.Routes;

public class GenerateRouteRequest
{
    public Guid PostmanId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly PlannedStartTime { get; set; }
}
