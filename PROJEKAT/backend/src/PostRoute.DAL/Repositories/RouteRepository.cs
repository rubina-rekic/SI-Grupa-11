using PostRoute.DAL.Entities;

namespace PostRoute.DAL.Repositories;

public class RouteRepository : IRouteRepository
{
    private readonly AppDbContext _context;

    public RouteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Route> CreateAsync(Route route, CancellationToken cancellationToken = default)
    {
        _context.Routes.Add(route);
        await _context.SaveChangesAsync(cancellationToken);
        return route;
    }
}