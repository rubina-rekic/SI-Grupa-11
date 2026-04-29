using Moq;
using PostRoute.BLL.Commands;
using PostRoute.BLL.Exceptions;
using PostRoute.BLL.Services;
using PostRoute.DAL.Entities;
using PostRoute.DAL.Repositories;
using PostRoute.Domain.Entities;

namespace PostRoute.BLL.Tests.Services;

public sealed class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly UserService _sut;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _sut = new UserService(_userRepositoryMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserDoesNotExist_ShouldReturnNull()
    {
        var userId = Guid.NewGuid();

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var result = await _sut.GetByIdAsync(userId, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserExists_ShouldReturnMappedUserModel()
    {
        var userId = Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            FirstName = "Amar",
            LastName = "Hodzic",
            Username = "amar.hodzic",
            Email = "amar.hodzic@posta.ba",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123"),
            Role = "PostalWorker",
            MustChangePassword = true,
            CreatedAt = DateTime.UtcNow,
        };

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _sut.GetByIdAsync(userId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Username, result.Username);
        Assert.Equal(user.Email, result.Email);
        Assert.Equal(user.Role, result.Role);
        Assert.Equal(user.MustChangePassword, result.MustChangePassword);
    }

    [Fact]
    public async Task CreateAsync_WhenEmailAlreadyExists_ShouldThrowInvalidOperationException()
    {
        var command = CreateValidCommand();

        _userRepositoryMock
            .Setup(x => x.EmailExistsAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.CreateAsync(command, CancellationToken.None));

        Assert.Equal($"Email '{command.Email}' je već u upotrebi.", exception.Message);

        _userRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenUsernameAlreadyExists_ShouldThrowInvalidOperationException()
    {
        var command = CreateValidCommand();

        _userRepositoryMock
            .Setup(x => x.EmailExistsAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _userRepositoryMock
            .Setup(x => x.UsernameExistsAsync(command.Username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.CreateAsync(command, CancellationToken.None));

        Assert.Equal($"Korisničko ime '{command.Username}' je već u upotrebi.", exception.Message);

        _userRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenRoleIsInvalid_ShouldThrowInvalidOperationException()
    {
        var command = new CreateUserCommand(
            "Amar",
            "Hodzic",
            "amar.hodzic",
            "amar.hodzic@posta.ba",
            "Password123",
            "InvalidRole");

        _userRepositoryMock
            .Setup(x => x.EmailExistsAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _userRepositoryMock
            .Setup(x => x.UsernameExistsAsync(command.Username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.CreateAsync(command, CancellationToken.None));

        Assert.Equal($"Nevažeća uloga: {command.Role}", exception.Message);

        _userRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenCommandIsValid_ShouldAddUser()
    {
        var command = CreateValidCommand();
        User? addedUser = null;

        _userRepositoryMock
            .Setup(x => x.EmailExistsAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _userRepositoryMock
            .Setup(x => x.UsernameExistsAsync(command.Username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _userRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Callback<User, CancellationToken>((user, _) => addedUser = user)
            .Returns(Task.CompletedTask);

        await _sut.CreateAsync(command, CancellationToken.None);

        Assert.NotNull(addedUser);
        Assert.NotEqual(Guid.Empty, addedUser.Id);
        Assert.Equal(command.FirstName, addedUser.FirstName);
        Assert.Equal(command.LastName, addedUser.LastName);
        Assert.Equal(command.Username, addedUser.Username);
        Assert.Equal(command.Email, addedUser.Email);
        Assert.Equal(command.Role, addedUser.Role);
        Assert.True(addedUser.MustChangePassword);
        Assert.False(string.IsNullOrWhiteSpace(addedUser.PasswordHash));
        Assert.NotEqual(command.Password, addedUser.PasswordHash);
        Assert.True(BCrypt.Net.BCrypt.Verify(command.Password, addedUser.PasswordHash));
    }

    [Fact]
    public async Task CreateAsync_WhenCommandIsValid_ShouldReturnMappedUserModel()
    {
        var command = CreateValidCommand();

        _userRepositoryMock
            .Setup(x => x.EmailExistsAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _userRepositoryMock
            .Setup(x => x.UsernameExistsAsync(command.Username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _userRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _sut.CreateAsync(command, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(command.Username, result.Username);
        Assert.Equal(command.Email, result.Email);
        Assert.Equal(command.Role, result.Role);
        Assert.True(result.MustChangePassword);
    }

    [Fact]
    public async Task CreateAsync_WhenEmailExists_ShouldNotCheckUsername()
    {
        var command = CreateValidCommand();

        _userRepositoryMock
            .Setup(x => x.EmailExistsAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.CreateAsync(command, CancellationToken.None));

        _userRepositoryMock.Verify(
            x => x.UsernameExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WhenUserDoesNotExist_ShouldThrowInvalidCredentialsException()
    {
        var email = "amar.hodzic@posta.ba";

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<InvalidCredentialsException>(() =>
            _sut.LoginAsync(email, "Password123", CancellationToken.None));
    }

    [Fact]
    public async Task LoginAsync_WhenUserIsLockedOut_ShouldThrowAccountLockedException()
    {
        var user = CreateUser("Password123");
        user.IsLockedOut = true;

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        await Assert.ThrowsAsync<AccountLockedException>(() =>
            _sut.LoginAsync(user.Email, "Password123", CancellationToken.None));

        _userRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordIsInvalid_ShouldIncrementFailedAttempts()
    {
        var user = CreateUser("Password123");
        user.FailedAttempts = 2;

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        await Assert.ThrowsAsync<InvalidCredentialsException>(() =>
            _sut.LoginAsync(user.Email, "WrongPassword123", CancellationToken.None));

        Assert.Equal(3, user.FailedAttempts);
        Assert.False(user.IsLockedOut);

        _userRepositoryMock.Verify(
            x => x.UpdateAsync(user, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordIsInvalidAndFailedAttemptsReachFive_ShouldLockUser()
    {
        var user = CreateUser("Password123");
        user.FailedAttempts = 4;

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        await Assert.ThrowsAsync<AccountLockedException>(() =>
            _sut.LoginAsync(user.Email, "WrongPassword123", CancellationToken.None));

        Assert.Equal(5, user.FailedAttempts);
        Assert.True(user.IsLockedOut);

        _userRepositoryMock.Verify(
            x => x.UpdateAsync(user, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordIsValid_ShouldResetFailedAttempts()
    {
        var user = CreateUser("Password123");
        user.FailedAttempts = 3;

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        await _sut.LoginAsync(user.Email, "Password123", CancellationToken.None);

        Assert.Equal(0, user.FailedAttempts);

        _userRepositoryMock.Verify(
            x => x.UpdateAsync(user, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordIsValid_ShouldReturnMappedUserModel()
    {
        var user = CreateUser("Password123");
        user.MustChangePassword = true;

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _sut.LoginAsync(user.Email, "Password123", CancellationToken.None);

        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Username, result.Username);
        Assert.Equal(user.Email, result.Email);
        Assert.Equal(user.Role, result.Role);
        Assert.Equal(user.MustChangePassword, result.MustChangePassword);
    }

    [Fact]
    public async Task ChangePasswordAsync_WhenUserDoesNotExist_ShouldThrowInvalidOperationException()
    {
        var email = "amar.hodzic@posta.ba";

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.ChangePasswordAsync(email, "Password123", "NewPassword123", CancellationToken.None));

        Assert.Equal("User not found.", exception.Message);
    }

    [Fact]
    public async Task ChangePasswordAsync_WhenCurrentPasswordIsIncorrect_ShouldThrowInvalidOperationException()
    {
        var user = CreateUser("Password123");

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.ChangePasswordAsync(user.Email, "WrongPassword123", "NewPassword123", CancellationToken.None));

        Assert.Equal("Current password is incorrect.", exception.Message);

        _userRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ChangePasswordAsync_WhenNewPasswordIsSameAsCurrentPassword_ShouldThrowInvalidOperationException()
    {
        var user = CreateUser("Password123");

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.ChangePasswordAsync(user.Email, "Password123", "Password123", CancellationToken.None));

        Assert.Equal("Nova lozinka mora biti različita od trenutne lozinke.", exception.Message);

        _userRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ChangePasswordAsync_WhenRequestIsValid_ShouldUpdatePasswordHash()
    {
        var user = CreateUser("Password123");
        var oldPasswordHash = user.PasswordHash;

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        await _sut.ChangePasswordAsync(user.Email, "Password123", "NewPassword123", CancellationToken.None);

        Assert.NotEqual(oldPasswordHash, user.PasswordHash);
        Assert.True(BCrypt.Net.BCrypt.Verify("NewPassword123", user.PasswordHash));
        Assert.False(BCrypt.Net.BCrypt.Verify("Password123", user.PasswordHash));

        _userRepositoryMock.Verify(
            x => x.UpdateAsync(user, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ChangePasswordAsync_WhenRequestIsValid_ShouldSetMustChangePasswordToFalse()
    {
        var user = CreateUser("Password123");
        user.MustChangePassword = true;

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        await _sut.ChangePasswordAsync(user.Email, "Password123", "NewPassword123", CancellationToken.None);

        Assert.False(user.MustChangePassword);

        _userRepositoryMock.Verify(
            x => x.UpdateAsync(user, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private static CreateUserCommand CreateValidCommand()
    {
        return new CreateUserCommand(
            "Amar",
            "Hodzic",
            "amar.hodzic",
            "amar.hodzic@posta.ba",
            "Password123",
            "PostalWorker");
    }

    private static User CreateUser(string password)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            FirstName = "Amar",
            LastName = "Hodzic",
            Username = "amar.hodzic",
            Email = "amar.hodzic@posta.ba",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = "PostalWorker",
            MustChangePassword = false,
            FailedAttempts = 0,
            IsLockedOut = false,
            CreatedAt = DateTime.UtcNow,
        };
    }
}