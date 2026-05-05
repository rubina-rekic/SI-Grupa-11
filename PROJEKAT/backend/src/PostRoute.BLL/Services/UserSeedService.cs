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
            new SeedUser("Admin", "User", "admin", "admin@mail.com", "Admin123!", UserRole.Administrator),
            new SeedUser("Postar", "User", "postar", "postar@mail.com", "Postar123!", UserRole.PostalWorker),
            new SeedUser("Postar", "User", "postar1", "postar1@mail.com", "Postar123!", UserRole.PostalWorker)
        };

        foreach (var seedUser in defaultUsers)
        {
            if (await _userRepository.EmailExistsAsync(seedUser.Email, cancellationToken)
                || await _userRepository.UsernameExistsAsync(seedUser.Username, cancellationToken))
            {
                continue;
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = seedUser.FirstName,
                LastName = seedUser.LastName,
                Username = seedUser.Username,
                Email = seedUser.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(seedUser.Password),
                Role = seedUser.Role,
                MustChangePassword = false,
                CreatedAt = DateTime.UtcNow,
                FailedAttempts = 0,
                IsLockedOut = false
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
