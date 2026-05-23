using PostRoute.BLL.Models.Routes;

namespace PostRoute.BLL.Services;

public interface IRouteService
{
    Task<RouteResponse> GenerateRouteAsync(GenerateRouteRequest request, CancellationToken cancellationToken = default);
    Task<RouteResponse?> GetRouteDetailsAsync(Guid routeId, CancellationToken cancellationToken = default);
    Task<RouteResponse?> GetPostmanAssignedRouteForTodayAsync(Guid postmanId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AvailablePostmanResponse>> GetAvailablePostmenAsync(Guid routeId, CancellationToken cancellationToken = default);
    Task<RouteResponse> AssignRouteAsync(Guid routeId, AssignRouteRequest request, string assignedBy, CancellationToken cancellationToken = default);
    Task<RouteResponse> ReorderRouteAsync(Guid routeId, ReorderRouteRequest request, string reorderedBy, CancellationToken cancellationToken = default);
}
