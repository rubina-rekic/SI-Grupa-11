namespace PostRoute.BLL.Services;

public interface IUserSeedService
{
    Task SeedDefaultUsersAsync(CancellationToken cancellationToken);
}
