using Moq;
using PostRoute.BLL.Commands;
using PostRoute.BLL.Services;
using PostRoute.DAL.Entities;
using PostRoute.DAL.Repositories;
using Xunit;

namespace PostRoute.BLL.Tests.Services;

/// <summary>
/// Unit testovi za PBI-017: Unos tipa i osnovnih podataka sandučića
/// </summary>
public sealed class MailboxServiceTestsPBI017
{
    private readonly Mock<IMailboxRepository> _mailboxRepositoryMock;
    private readonly MailboxService _sut;

    public MailboxServiceTestsPBI017()
    {
        _mailboxRepositoryMock = new Mock<IMailboxRepository>();
        _sut = new MailboxService(_mailboxRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateMailboxWithValidData()
    {
        // Arrange
        var command = new CreateMailboxCommand(
            "TEST001",
            "Test Address 1",
            43.8563m,
            18.4131m,
            MailboxType.WallSmall,
            100,
            2024
        );

        var expectedMailbox = new Mailbox
        {
            Id = Guid.NewGuid(),
            SerialNumber = command.SerialNumber,
            Address = command.Address,
            Latitude = command.Latitude,
            Longitude = command.Longitude,
            Type = command.Type,
            Capacity = command.Capacity,
            InstallationYear = command.InstallationYear,
            Notes = null
        };

        _mailboxRepositoryMock
            .Setup(x => x.SerialNumberExistsAsync(command.SerialNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mailboxRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Mailbox>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedMailbox);

        // Act
        var result = await _sut.CreateAsync(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("TEST001", result.SerialNumber);
        Assert.Equal("Test Address 1", result.Address);
        Assert.Equal(43.8563m, result.Latitude);
        Assert.Equal(18.4131m, result.Longitude);
        Assert.Equal(MailboxType.WallSmall, result.Type);
        Assert.Equal(100, result.Capacity);
        Assert.Equal(2024, result.InstallationYear);

        _mailboxRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Mailbox>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowExceptionWhenSerialNumberExists()
    {
        // Arrange
        var command = new CreateMailboxCommand(
            "EXIST001",
            "New Address",
            43.8563m,
            18.4131m,
            MailboxType.StandaloneLarge,
            150,
            2024
        );

        _mailboxRepositoryMock
            .Setup(x => x.SerialNumberExistsAsync(command.SerialNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CreateAsync(command, CancellationToken.None));
        Assert.Contains("već postoji", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(-91, 0, MailboxType.WallSmall, 100, 2024)] // Invalid latitude
    [InlineData(91, 0, MailboxType.WallSmall, 100, 2024)]  // Invalid latitude
    [InlineData(0, -181, MailboxType.WallSmall, 100, 2024)] // Invalid longitude
    [InlineData(0, 181, MailboxType.WallSmall, 100, 2024)]  // Invalid longitude
    [InlineData(43.8563, 18.4131, MailboxType.WallSmall, 0, 2024)] // Invalid capacity
    [InlineData(43.8563, 18.4131, MailboxType.WallSmall, 100, 1899)] // Invalid year
    [InlineData(43.8563, 18.4131, MailboxType.WallSmall, 100, 2101)] // Invalid year
    public async Task CreateAsync_ShouldThrowExceptionWithInvalidCoordinatesOrData(
        decimal latitude, decimal longitude, MailboxType type, int capacity, int year)
    {
        // Arrange
        var command = new CreateMailboxCommand(
            "INVALID001",
            "Invalid Address",
            latitude,
            longitude,
            type,
            capacity,
            year
        );

        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(() => _sut.CreateAsync(command, CancellationToken.None));
    }

    [Fact]
    public async Task CreateAsync_ShouldHandleAllMailboxTypes()
    {
        // Arrange
        var mailboxTypes = new[]
        {
            MailboxType.WallSmall,
            MailboxType.StandaloneLarge,
            MailboxType.IndoorResidential,
            MailboxType.SpecialPriority
        };

        foreach (var mailboxType in mailboxTypes)
        {
            var command = new CreateMailboxCommand(
                $"{mailboxType}_TEST",
                $"Test Address for {mailboxType}",
                43.8563m,
                18.4131m,
                mailboxType,
                100,
                2024
            );

            var expectedMailbox = new Mailbox
            {
                Id = Guid.NewGuid(),
                SerialNumber = command.SerialNumber,
                Address = command.Address,
                Latitude = command.Latitude,
                Longitude = command.Longitude,
                Type = command.Type,
                Capacity = command.Capacity,
                InstallationYear = command.InstallationYear,
                Notes = null
            };

            _mailboxRepositoryMock
                .Setup(x => x.SerialNumberExistsAsync(command.SerialNumber, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _mailboxRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Mailbox>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedMailbox);

            // Act
            var result = await _sut.CreateAsync(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(mailboxType, result.Type);
        }
    }

    [Fact]
    public async Task CreateAsync_ShouldHandleNullNotes()
    {
        // Arrange
        var command = new CreateMailboxCommand(
            "NULL_NOTES",
            "Test Address",
            43.8563m,
            18.4131m,
            MailboxType.WallSmall,
            100,
            2024
        );

        var expectedMailbox = new Mailbox
        {
            Id = Guid.NewGuid(),
            SerialNumber = command.SerialNumber,
            Address = command.Address,
            Latitude = command.Latitude,
            Longitude = command.Longitude,
            Type = command.Type,
            Capacity = command.Capacity,
            InstallationYear = command.InstallationYear,
            Notes = null
        };

        _mailboxRepositoryMock
            .Setup(x => x.SerialNumberExistsAsync(command.SerialNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mailboxRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Mailbox>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedMailbox);

        // Act
        var result = await _sut.CreateAsync(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Notes);
    }

    [Fact]
    public async Task CreateAsync_ShouldHandleEmptyNotes()
    {
        // Arrange
        var command = new CreateMailboxCommand(
            "EMPTY_NOTES",
            "Test Address",
            43.8563m,
            18.4131m,
            MailboxType.WallSmall,
            100,
            2024
        );

        var expectedMailbox = new Mailbox
        {
            Id = Guid.NewGuid(),
            SerialNumber = command.SerialNumber,
            Address = command.Address,
            Latitude = command.Latitude,
            Longitude = command.Longitude,
            Type = command.Type,
            Capacity = command.Capacity,
            InstallationYear = command.InstallationYear,
            Notes = ""
        };

        _mailboxRepositoryMock
            .Setup(x => x.SerialNumberExistsAsync(command.SerialNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mailboxRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Mailbox>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedMailbox);

        // Act
        var result = await _sut.CreateAsync(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("", result.Notes);
    }

    [Fact]
    public async Task CreateAsync_ShouldTrimWhitespaceFromStrings()
    {
        // Arrange
        var command = new CreateMailboxCommand(
            "  WHITESPACE_TEST  ",
            "  Test Address with whitespace  ",
            43.8563m,
            18.4131m,
            MailboxType.WallSmall,
            100,
            2024
        );

        var expectedMailbox = new Mailbox
        {
            Id = Guid.NewGuid(),
            SerialNumber = "WHITESPACE_TEST",
            Address = "Test Address with whitespace",
            Latitude = command.Latitude,
            Longitude = command.Longitude,
            Type = command.Type,
            Capacity = command.Capacity,
            InstallationYear = command.InstallationYear,
            Notes = "Test notes with whitespace"
        };

        _mailboxRepositoryMock
            .Setup(x => x.SerialNumberExistsAsync(command.SerialNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mailboxRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Mailbox>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedMailbox);

        // Act
        var result = await _sut.CreateAsync(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("WHITESPACE_TEST", result.SerialNumber);
        Assert.Equal("Test Address with whitespace", result.Address);
        Assert.Equal("Test notes with whitespace", result.Notes);
    }

    [Fact]
    public async Task CreateAsync_ShouldCallRepositoryMethods()
    {
        // Arrange
        var command = new CreateMailboxCommand(
            "LOG_TEST",
            "Test Address",
            43.8563m,
            18.4131m,
            MailboxType.WallSmall,
            100,
            2024
        );

        var expectedMailbox = new Mailbox
        {
            Id = Guid.NewGuid(),
            SerialNumber = command.SerialNumber,
            Address = command.Address,
            Latitude = command.Latitude,
            Longitude = command.Longitude,
            Type = command.Type,
            Capacity = command.Capacity,
            InstallationYear = command.InstallationYear,
            Notes = null
        };

        _mailboxRepositoryMock
            .Setup(x => x.SerialNumberExistsAsync(command.SerialNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mailboxRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Mailbox>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedMailbox);

        // Act
        await _sut.CreateAsync(command, CancellationToken.None);

        // Assert - Verify that repository methods were called
        _mailboxRepositoryMock.Verify(x => x.SerialNumberExistsAsync(command.SerialNumber, It.IsAny<CancellationToken>()), Times.Once);
        _mailboxRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Mailbox>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
