using PostRoute.DAL.Entities;

namespace PostRoute.DAL.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken);
}
