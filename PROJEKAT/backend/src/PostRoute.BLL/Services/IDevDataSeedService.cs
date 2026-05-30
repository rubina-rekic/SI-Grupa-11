namespace PostRoute.BLL.Services;

public interface IDevDataSeedService
{
    Task SeedArchiveRouteAsync(CancellationToken cancellationToken);
}
