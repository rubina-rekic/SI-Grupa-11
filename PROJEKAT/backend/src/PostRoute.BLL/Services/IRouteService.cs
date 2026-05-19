using PostRoute.BLL.Models.Routes;

namespace PostRoute.BLL.Services;

public interface IRouteService
{
    Task<RouteResponse> GenerateRouteAsync(GenerateRouteRequest request, CancellationToken cancellationToken = default);
    Task<RouteResponse?> GetRouteDetailsAsync(Guid routeId, CancellationToken cancellationToken = default);
}