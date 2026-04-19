using PostRoute.BLL.Models;

namespace PostRoute.BLL.Services;

public interface IUserService
{
    Task<UserModel?> GetByIdAsync(Guid userId, CancellationToken cancellationToken);
}
