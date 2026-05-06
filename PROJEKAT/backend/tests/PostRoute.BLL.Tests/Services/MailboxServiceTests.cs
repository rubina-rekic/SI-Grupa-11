using Moq;
using PostRoute.BLL.Commands;
using PostRoute.BLL.Services;
using PostRoute.DAL.Entities;
using PostRoute.DAL.Repositories;

namespace PostRoute.BLL.Tests.Services;

public sealed class MailboxServiceTests
{
    private readonly Mock<IMailboxRepository> _mailboxRepositoryMock;
    private readonly MailboxService _sut;

    public MailboxServiceTests()
    {
        _mailboxRepositoryMock = new Mock<IMailboxRepository>();
        _sut = new MailboxService(_mailboxRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WhenSerialNumberExists_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var command = new CreateMailboxCommand(
            "SN001",
            "Test Address",
            43.8563m,
            18.4131m,
            MailboxType.WallSmall,
            100,
            2023
        );

        _mailboxRepositoryMock
            .Setup(x => x.SerialNumberExistsAsync(command.SerialNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.CreateAsync(command, CancellationToken.None));

        Assert.Equal("Sandučić sa serijskim brojem 'SN001' već postoji.", exception.Message);
    }

    [Fact]
    public async Task CreateAsync_WhenLatitudeIsOutOfRange_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var command = new CreateMailboxCommand(
            "SN001",
            "Test Address",
            91.0m, // Invalid latitude
            18.4131m,
            MailboxType.WallSmall,
            100,
            2023
        );

        _mailboxRepositoryMock
            .Setup(x => x.SerialNumberExistsAsync(command.SerialNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.CreateAsync(command, CancellationToken.None));

        Assert.Equal("Latitude mora biti između -90 i 90.", exception.Message);
    }

    [Fact]
    public async Task CreateAsync_WhenLongitudeIsOutOfRange_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var command = new CreateMailboxCommand(
            "SN001",
            "Test Address",
            43.8563m,
            181.0m, // Invalid longitude
            MailboxType.WallSmall,
            100,
            2023
        );

        _mailboxRepositoryMock
            .Setup(x => x.SerialNumberExistsAsync(command.SerialNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.CreateAsync(command, CancellationToken.None));

        Assert.Equal("Longitude mora biti između -180 i 180.", exception.Message);
    }

    [Fact]
    public async Task CreateAsync_WhenCapacityIsInvalid_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var command = new CreateMailboxCommand(
            "SN001",
            "Test Address",
            43.8563m,
            18.4131m,
            MailboxType.WallSmall,
            0, // Invalid capacity
            2023
        );

        _mailboxRepositoryMock
            .Setup(x => x.SerialNumberExistsAsync(command.SerialNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.CreateAsync(command, CancellationToken.None));

        Assert.Equal("Kapacitet mora biti veći od 0.", exception.Message);
    }

    [Fact]
    public async Task CreateAsync_WhenInstallationYearIsInvalid_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var command = new CreateMailboxCommand(
            "SN001",
            "Test Address",
            43.8563m,
            18.4131m,
            MailboxType.WallSmall,
            100,
            1899 // Invalid installation year
        );

        _mailboxRepositoryMock
            .Setup(x => x.SerialNumberExistsAsync(command.SerialNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.CreateAsync(command, CancellationToken.None));

        Assert.Contains("Godina instalacije mora biti između 1900 i", exception.Message);
    }

    [Fact]
    public async Task CreateAsync_WhenValidData_ShouldCreateMailbox()
    {
        // Arrange
        var command = new CreateMailboxCommand(
            "SN001",
            "Test Address 123",
            43.8563m,
            18.4131m,
            MailboxType.StandaloneLarge,
            150,
            2023,
            "Test notes"
        );

        _mailboxRepositoryMock
            .Setup(x => x.SerialNumberExistsAsync(command.SerialNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        Mailbox? createdMailbox = null;
        _mailboxRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Mailbox>(), It.IsAny<CancellationToken>()))
            .Callback<Mailbox, CancellationToken>((mailbox, _) => createdMailbox = mailbox)
            .ReturnsAsync((Mailbox mailbox, CancellationToken _) => mailbox);

        // Act
        var result = await _sut.CreateAsync(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.SerialNumber, result.SerialNumber);
        Assert.Equal(command.Address, result.Address);
        Assert.Equal(command.Latitude, result.Latitude);
        Assert.Equal(command.Longitude, result.Longitude);
        Assert.Equal(command.Type, result.Type);
        Assert.Equal(command.Capacity, result.Capacity);
        Assert.Equal(command.InstallationYear, result.InstallationYear);
        Assert.Equal(command.Notes, result.Notes);
        Assert.True(result.CreatedAt > DateTime.MinValue);
        Assert.True(result.UpdatedAt > DateTime.MinValue);

        _mailboxRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Mailbox>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WhenMailboxExists_ShouldReturnMailbox()
    {
        // Arrange
        var mailboxId = Guid.NewGuid();
        var expectedMailbox = new Mailbox
        {
            Id = mailboxId,
            SerialNumber = "SN001",
            Address = "Test Address",
            Latitude = 43.8563m,
            Longitude = 18.4131m,
            Type = MailboxType.WallSmall,
            Capacity = 100,
            InstallationYear = 2023,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mailboxRepositoryMock
            .Setup(x => x.GetByIdAsync(mailboxId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedMailbox);

        // Act
        var result = await _sut.GetByIdAsync(mailboxId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedMailbox.Id, result.Id);
        Assert.Equal(expectedMailbox.SerialNumber, result.SerialNumber);
    }

    [Fact]
    public async Task GetByIdAsync_WhenMailboxDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var mailboxId = Guid.NewGuid();

        _mailboxRepositoryMock
            .Setup(x => x.GetByIdAsync(mailboxId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Mailbox?)null);

        // Act
        var result = await _sut.GetByIdAsync(mailboxId, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetBySerialNumberAsync_WhenMailboxExists_ShouldReturnMailbox()
    {
        // Arrange
        var serialNumber = "SN001";
        var expectedMailbox = new Mailbox
        {
            Id = Guid.NewGuid(),
            SerialNumber = serialNumber,
            Address = "Test Address",
            Latitude = 43.8563m,
            Longitude = 18.4131m,
            Type = MailboxType.WallSmall,
            Capacity = 100,
            InstallationYear = 2023,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mailboxRepositoryMock
            .Setup(x => x.GetBySerialNumberAsync(serialNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedMailbox);

        // Act
        var result = await _sut.GetBySerialNumberAsync(serialNumber, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedMailbox.SerialNumber, result.SerialNumber);
    }

    [Fact]
    public async Task SerialNumberExistsAsync_ShouldReturnRepositoryResult()
    {
        // Arrange
        var serialNumber = "SN001";
        var expectedResult = true;

        _mailboxRepositoryMock
            .Setup(x => x.SerialNumberExistsAsync(serialNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _sut.SerialNumberExistsAsync(serialNumber, CancellationToken.None);

        // Assert
        Assert.Equal(expectedResult, result);
        _mailboxRepositoryMock.Verify(x => x.SerialNumberExistsAsync(serialNumber, It.IsAny<CancellationToken>()), Times.Once);
    }
}
