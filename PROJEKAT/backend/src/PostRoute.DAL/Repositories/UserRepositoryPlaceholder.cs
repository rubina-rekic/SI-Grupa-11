using PostRoute.DAL.Entities;

namespace PostRoute.DAL.Repositories;

public sealed class UserRepositoryPlaceholder : IUserRepository
{
    public Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = new User(
            userId,
            "placeholder.user",
            "placeholder.user@postroute.local",
            "Dispatcher");

        return Task.FromResult<User?>(user);
    }
}
