using System.Threading;
using System.Threading.Tasks;

namespace PostRoute.BLL.Services;

public interface IUserSeedService
{
    Task SeedDefaultUsersAsync(CancellationToken cancellationToken);
}
