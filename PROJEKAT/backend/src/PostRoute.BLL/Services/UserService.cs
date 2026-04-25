using PostRoute.BLL.Commands;
using PostRoute.BLL.Models;
using PostRoute.DAL.Entities;
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

    public async Task<UserModel> CreateAsync(CreateUserCommand command, CancellationToken cancellationToken)
    {
        if (await _userRepository.EmailExistsAsync(command.Email, cancellationToken))
            throw new InvalidOperationException($"Email '{command.Email}' je već u upotrebi.");

        if (await _userRepository.UsernameExistsAsync(command.Username, cancellationToken))
            throw new InvalidOperationException($"Korisničko ime '{command.Username}' je već u upotrebi.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = command.FirstName,
            LastName = command.LastName,
            Username = command.Username,
            Email = command.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.Password),
            Role = "PostalWorker",
            MustChangePassword = true,
            CreatedAt = DateTime.UtcNow,
        };

        await _userRepository.AddAsync(user, cancellationToken);

        return new UserModel(user.Id, user.Username, user.Email, user.Role);
    }
}
