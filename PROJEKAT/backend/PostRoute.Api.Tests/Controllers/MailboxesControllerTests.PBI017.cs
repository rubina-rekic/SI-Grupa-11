using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PostRoute.Api.Contracts.Mailboxes;
using PostRoute.BLL.Services;
using PostRoute.DAL.Entities;
using Xunit;

namespace PostRoute.Api.Tests.Controllers;

/// <summary>
/// Unit testovi za PBI-017: MailboxesController API endpoints
/// </summary>
public sealed class MailboxesControllerTestsPBI017
{
    private readonly Mock<IMailboxService> _mailboxServiceMock;
    private readonly Mock<ILogger<MailboxesController>> _loggerMock;
    private readonly MailboxesController _sut;

    public MailboxesControllerTestsPBI017()
    {
        _mailboxServiceMock = new Mock<IMailboxService>();
        _loggerMock = new Mock<ILogger<MailboxesController>>();
        _sut = new MailboxesController(_mailboxServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnCreatedResult_WhenValidRequest()
    {
        // Arrange
        var request = new CreateMailboxRequest
        {
            SerialNumber = "TEST001",
            Address = "Test Address 1",
            Latitude = 43.8563m,
            Longitude = 18.4131m,
            Type = MailboxType.WallSmall,
            Capacity = 100,
            InstallationYear = 2024,
            Notes = "Test notes"
        };

        var expectedMailbox = new Mailbox(
            Guid.NewGuid(),
            request.SerialNumber,
            request.Address,
            request.Latitude,
            request.Longitude,
            request.Type,
            request.Capacity,
            request.InstallationYear,
            request.Notes
        );

        _mailboxServiceMock
            .Setup(x => x.CreateAsync(It.IsAny<CreateMailboxCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedMailbox);

        // Act
        var result = await _sut.CreateAsync(request, CancellationToken.None);

        // Assert
        var createdResult = Assert.IsType<CreatedResult>(result.Result);
        Assert.Equal($"/api/mailboxes/{expectedMailbox.Id}", createdResult.Location);
        
        var response = Assert.IsType<MailboxResponse>(createdResult.Value);
        Assert.Equal(expectedMailbox.Id, response.Id);
        Assert.Equal(expectedMailbox.SerialNumber, response.SerialNumber);
        Assert.Equal(expectedMailbox.Address, response.Address);
        Assert.Equal(expectedMailbox.Latitude, response.Latitude);
        Assert.Equal(expectedMailbox.Longitude, response.Longitude);
        Assert.Equal(expectedMailbox.Type, response.Type);
        Assert.Equal(expectedMailbox.Capacity, response.Capacity);
        Assert.Equal(expectedMailbox.InstallationYear, response.InstallationYear);
        Assert.Equal(expectedMailbox.Notes, response.Notes);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnBadRequest_WhenInvalidRequest()
    {
        // Arrange
        var request = new CreateMailboxRequest
        {
            SerialNumber = "", // Invalid: empty serial number
            Address = "Test Address",
            Latitude = 43.8563m,
            Longitude = 18.4131m,
            Type = MailboxType.WallSmall,
            Capacity = 100,
            InstallationYear = 2024,
            Notes = "Test notes"
        };

        _sut.ModelState.AddModelError("SerialNumber", "Serijski broj je obavezan");

        // Act
        var result = await _sut.CreateAsync(request, CancellationToken.None);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnConflict_WhenSerialNumberExists()
    {
        // Arrange
        var request = new CreateMailboxRequest
        {
            SerialNumber = "EXIST001",
            Address = "Test Address",
            Latitude = 43.8563m,
            Longitude = 18.4131m,
            Type = MailboxType.WallSmall,
            Capacity = 100,
            InstallationYear = 2024,
            Notes = "Test notes"
        };

        _mailboxServiceMock
            .Setup(x => x.CreateAsync(It.IsAny<CreateMailboxCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Sandučić sa ovim serijskim brojem već postoji"));

        // Act
        var result = await _sut.CreateAsync(request, CancellationToken.None);

        // Assert
        var conflictResult = Assert.IsType<ConflictObjectResult>(result.Result);
        Assert.Equal(409, conflictResult.StatusCode);
        Assert.Contains("već postoji", conflictResult.Value.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnInternalServerError_WhenServiceThrowsException()
    {
        // Arrange
        var request = new CreateMailboxRequest
        {
            SerialNumber = "ERROR001",
            Address = "Test Address",
            Latitude = 43.8563m,
            Longitude = 18.4131m,
            Type = MailboxType.WallSmall,
            Capacity = 100,
            InstallationYear = 2024,
            Notes = "Test notes"
        };

        _mailboxServiceMock
            .Setup(x => x.CreateAsync(It.IsAny<CreateMailboxCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _sut.CreateAsync(request, CancellationToken.None);

        // Assert
        var errorResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, errorResult.StatusCode);
    }

    [Theory]
    [InlineData(-91, 0, MailboxType.WallSmall, 100, 2024)] // Invalid latitude
    [InlineData(91, 0, MailboxType.WallSmall, 100, 2024)]  // Invalid latitude
    [InlineData(0, -181, MailboxType.WallSmall, 100, 2024)] // Invalid longitude
    [InlineData(0, 181, MailboxType.WallSmall, 100, 2024)]  // Invalid longitude
    [InlineData(43.8563, 18.4131, MailboxType.WallSmall, 0, 2024)] // Invalid capacity
    [InlineData(43.8563, 18.4131, MailboxType.WallSmall, 100, 1899)] // Invalid year
    [InlineData(43.8563, 18.4131, MailboxType.WallSmall, 100, 2101)] // Invalid year
    public async Task CreateAsync_ShouldReturnBadRequest_WhenInvalidData(
        decimal latitude, decimal longitude, MailboxType type, int capacity, int year)
    {
        // Arrange
        var request = new CreateMailboxRequest
        {
            SerialNumber = "INVALID001",
            Address = "Test Address",
            Latitude = latitude,
            Longitude = longitude,
            Type = type,
            Capacity = capacity,
            InstallationYear = year,
            Notes = "Test notes"
        };

        // Act
        var result = await _sut.CreateAsync(request, CancellationToken.None);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(400, badRequestResult.StatusCode);
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
            var request = new CreateMailboxRequest
            {
                SerialNumber = $"{mailboxType}_TEST",
                Address = $"Test Address for {mailboxType}",
                Latitude = 43.8563m,
                Longitude = 18.4131m,
                Type = mailboxType,
                Capacity = 100,
                InstallationYear = 2024,
                Notes = $"Test notes for {mailboxType}"
            };

            var expectedMailbox = new Mailbox(
                Guid.NewGuid(),
                request.SerialNumber,
                request.Address,
                request.Latitude,
                request.Longitude,
                request.Type,
                request.Capacity,
                request.InstallationYear,
                request.Notes
            );

            _mailboxServiceMock
                .Setup(x => x.CreateAsync(It.IsAny<CreateMailboxCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedMailbox);

            // Act
            var result = await _sut.CreateAsync(request, CancellationToken.None);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result.Result);
            var response = Assert.IsType<MailboxResponse>(createdResult.Value);
            Assert.Equal(mailboxType, response.Type);
        }
    }

    [Fact]
    public async Task CreateAsync_ShouldHandleNullNotes()
    {
        // Arrange
        var request = new CreateMailboxRequest
        {
            SerialNumber = "NULL_NOTES",
            Address = "Test Address",
            Latitude = 43.8563m,
            Longitude = 18.4131m,
            Type = MailboxType.WallSmall,
            Capacity = 100,
            InstallationYear = 2024,
            Notes = null
        };

        var expectedMailbox = new Mailbox(
            Guid.NewGuid(),
            request.SerialNumber,
            request.Address,
            request.Latitude,
            request.Longitude,
            request.Type,
            request.Capacity,
            request.InstallationYear,
            request.Notes
        );

        _mailboxServiceMock
            .Setup(x => x.CreateAsync(It.IsAny<CreateMailboxCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedMailbox);

        // Act
        var result = await _sut.CreateAsync(request, CancellationToken.None);

        // Assert
        var createdResult = Assert.IsType<CreatedResult>(result.Result);
        var response = Assert.IsType<MailboxResponse>(createdResult.Value);
        Assert.Null(response.Notes);
    }

    [Fact]
    public async Task CheckSerialNumberExists_ShouldReturnTrue_WhenSerialNumberExists()
    {
        // Arrange
        var serialNumber = "EXIST001";
        _mailboxServiceMock
            .Setup(x => x.SerialNumberExistsAsync(serialNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.CheckSerialNumberExists(serialNumber);

        // Assert
        var okResult = Assert.IsType<Ok<bool>>(result);
        Assert.True(okResult.Value);
    }

    [Fact]
    public async Task CheckSerialNumberExists_ShouldReturnFalse_WhenSerialNumberDoesNotExist()
    {
        // Arrange
        var serialNumber = "NEW001";
        _mailboxServiceMock
            .Setup(x => x.SerialNumberExistsAsync(serialNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _sut.CheckSerialNumberExists(serialNumber);

        // Assert
        var okResult = Assert.IsType<Ok<bool>>(result);
        Assert.False(okResult.Value);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllMailboxes()
    {
        // Arrange
        var mailboxes = new List<Mailbox>
        {
            new(Guid.NewGuid(), "TEST001", "Address 1", 43.8563m, 18.4131m, MailboxType.WallSmall, 100, 2024, "Notes 1"),
            new(Guid.NewGuid(), "TEST002", "Address 2", 43.8563m, 18.4131m, MailboxType.StandaloneLarge, 150, 2024, "Notes 2")
        };

        _mailboxServiceMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(mailboxes);

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        var okResult = Assert.IsType<Ok<IEnumerable<MailboxResponse>>>(result);
        var responses = okResult.Value.ToList();
        Assert.Equal(2, responses.Count);
        Assert.All(responses, r =>
        {
            Assert.NotNull(r.Id);
            Assert.NotNull(r.SerialNumber);
            Assert.NotNull(r.Address);
        });
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnMailbox_WhenMailboxExists()
    {
        // Arrange
        var mailboxId = Guid.NewGuid();
        var mailbox = new Mailbox(
            mailboxId,
            "TEST001",
            "Test Address",
            43.8563m,
            18.4131m,
            MailboxType.WallSmall,
            100,
            2024,
            "Test notes"
        );

        _mailboxServiceMock
            .Setup(x => x.GetByIdAsync(mailboxId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mailbox);

        // Act
        var result = await _sut.GetByIdAsync(mailboxId);

        // Assert
        var okResult = Assert.IsType<Ok<MailboxResponse>>(result);
        var response = okResult.Value;
        Assert.Equal(mailboxId, response.Id);
        Assert.Equal("TEST001", response.SerialNumber);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNotFound_WhenMailboxDoesNotExist()
    {
        // Arrange
        var mailboxId = Guid.NewGuid();
        _mailboxServiceMock
            .Setup(x => x.GetByIdAsync(mailboxId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Mailbox?)null);

        // Act
        var result = await _sut.GetByIdAsync(mailboxId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundResult>(result);
        Assert.Equal(404, notFoundResult.StatusCode);
    }
}
