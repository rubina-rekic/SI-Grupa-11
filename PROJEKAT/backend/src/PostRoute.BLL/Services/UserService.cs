using PostRoute.BLL.Models;
using PostRoute.DAL.Repositories;

namespace PostRoute.BLL.Services;

public sealed class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserModel?> GetByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return null;
        }

        return new UserModel(user.Id, user.Username, user.Email, user.Role);
    }
}
