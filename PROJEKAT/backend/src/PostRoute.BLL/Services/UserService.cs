using PostRoute.BLL.Commands;
using PostRoute.BLL.Models;
using PostRoute.DAL.Entities;
using PostRoute.DAL.Repositories;
using PostRoute.Domain.Entities;

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

        return new UserModel(user.Id, user.Username, user.Email, user.Role, user.MustChangePassword);
    }

    public async Task<UserModel> CreateAsync(CreateUserCommand command, CancellationToken cancellationToken)
    {
        if (await _userRepository.EmailExistsAsync(command.Email, cancellationToken))
            throw new InvalidOperationException($"Email '{command.Email}' je već u upotrebi.");

        if (await _userRepository.UsernameExistsAsync(command.Username, cancellationToken))
            throw new InvalidOperationException($"Korisničko ime '{command.Username}' je već u upotrebi.");

        if (!UserRole.IsValidRole(command.Role))
            throw new InvalidOperationException($"Nevažeća uloga: {command.Role}");

        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = command.FirstName,
            LastName = command.LastName,
            Username = command.Username,
            Email = command.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.Password),
            Role = command.Role,
            MustChangePassword = true,
            CreatedAt = DateTime.UtcNow,
        };

        await _userRepository.AddAsync(user, cancellationToken);

        return new UserModel(user.Id, user.Username, user.Email, user.Role, user.MustChangePassword);
    }

    public async Task<UserModel> LoginAsync(string email, string password, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

        if (user is null || user.IsLockedOut)
            throw new InvalidOperationException("Invalid credentials or account is locked.");

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            user.FailedAttempts++;

            if (user.FailedAttempts >= 5)
                user.IsLockedOut = true;

            await _userRepository.UpdateAsync(user, cancellationToken);
            throw new InvalidOperationException("Invalid credentials.");
        }

        user.FailedAttempts = 0;
        await _userRepository.UpdateAsync(user, cancellationToken);

        return new UserModel(user.Id, user.Username, user.Email, user.Role, user.MustChangePassword);
    }

    public async Task ChangePasswordAsync(
        string email,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

        if (user is null)
            throw new InvalidOperationException("User not found.");

        if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
            throw new InvalidOperationException("Current password is incorrect.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        user.MustChangePassword = false;

        await _userRepository.UpdateAsync(user, cancellationToken);
    }
}


