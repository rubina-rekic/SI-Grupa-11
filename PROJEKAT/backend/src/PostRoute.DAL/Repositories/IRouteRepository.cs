using PostRoute.DAL.Entities;

namespace PostRoute.DAL.Repositories;

public interface IRouteRepository
{
    Task<Route> CreateAsync(Route route, CancellationToken cancellationToken = default);
}