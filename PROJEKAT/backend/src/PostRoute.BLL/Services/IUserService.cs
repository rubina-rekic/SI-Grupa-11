using PostRoute.BLL.Commands;
using PostRoute.BLL.Models;

namespace PostRoute.BLL.Services;

public interface IUserService
{
    Task<UserModel?> GetByIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<UserModel> CreateAsync(CreateUserCommand command, CancellationToken cancellationToken);

    Task<UserModel> LoginAsync(string email, string password, CancellationToken cancellationToken);
    Task ChangePasswordAsync(string email, string newPassword, CancellationToken cancellationToken
);
}
