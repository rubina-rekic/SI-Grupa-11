using PostRoute.DAL.Entities;
using PostRoute.DAL.Repositories;
using PostRoute.Domain.Entities;

namespace PostRoute.BLL.Services;

public sealed class UserSeedService : IUserSeedService
{
    private readonly IUserRepository _userRepository;

    public UserSeedService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task SeedDefaultUsersAsync(CancellationToken cancellationToken)
    {
        var defaultUsers = new[]
        {
            new SeedUser("Admin", "User", "admin", "admin@posta.ba", "Admin123!", UserRole.Administrator),
            new SeedUser("Postar", "Jedan", "postar1", "postar1@posta.ba", "Postar123!", UserRole.PostalWorker),
            new SeedUser("Postar", "Dva", "postar2", "postar2@posta.ba", "Postar123!", UserRole.PostalWorker),
        };

        foreach (var seed in defaultUsers)
        {
            if (await _userRepository.EmailExistsAsync(seed.Email, cancellationToken)
                || await _userRepository.UsernameExistsAsync(seed.Username, cancellationToken))
            {
                continue;
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = seed.FirstName,
                LastName = seed.LastName,
                Username = seed.Username,
                Email = seed.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(seed.Password),
                Role = seed.Role,
                MustChangePassword = false,
                CreatedAt = DateTime.UtcNow,
                FailedAttempts = 0,
                IsLockedOut = false,
            };

            await _userRepository.AddAsync(user, cancellationToken);
        }
    }

    private sealed record SeedUser(
        string FirstName,
        string LastName,
        string Username,
        string Email,
        string Password,
        string Role);
}
