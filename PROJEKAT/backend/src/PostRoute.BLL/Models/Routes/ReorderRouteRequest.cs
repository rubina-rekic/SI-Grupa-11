namespace PostRoute.BLL.Models.Routes;

public class ReorderRouteRequest
{
    public List<ReorderItem> Items { get; set; } = new();
}

public class ReorderItem
{
    public Guid RouteItemId { get; set; }
    public int NewOrder { get; set; }
}
