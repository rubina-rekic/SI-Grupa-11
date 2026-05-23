using FluentAssertions;
using Moq;
using PostRoute.BLL.Models.Routes;
using PostRoute.BLL.Services;
using PostRoute.DAL.Entities;
using PostRoute.DAL.Repositories;
using PostRoute.Domain.Entities;

namespace PostRoute.BLL.Tests.Services;

public class RouteServiceTestsPBI026
{
    private readonly Mock<IMailboxRepository> _mailboxRepositoryMock = new();
    private readonly Mock<IRouteRepository> _routeRepositoryMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly RouteService _sut;

    public RouteServiceTestsPBI026()
    {
        _sut = new RouteService(
            _mailboxRepositoryMock.Object,
            _routeRepositoryMock.Object,
            _userRepositoryMock.Object);
    }

    private static Mailbox MakeMailbox(decimal lat = 43.85m, decimal lng = 18.41m) => new()
    {
        Id = Guid.NewGuid(),
        SerialNumber = "SN001",
        Address = "Test adresa",
        Latitude = lat,
        Longitude = lng,
        Priority = MailboxPriority.Visok,
        IsAlwaysAvailable = true,
        IsActive = true,
        Type = MailboxType.StandaloneLarge,
        Status = MailboxStatus.Prazan,
        Capacity = 100,
        InstallationYear = 2020
    };

    private static Route MakeRoute(RouteStatus status, Guid? postmanId = null, DateOnly? date = null) => new()
    {
        Id = Guid.NewGuid(),
        PostmanId = postmanId ?? Guid.NewGuid(),
        Date = date ?? DateOnly.FromDateTime(DateTime.UtcNow),
        PlannedStartTime = new TimeOnly(8, 0),
        PlannedEndTime = new TimeOnly(10, 0),
        TotalDistanceKm = 12.5m,
        TotalDurationMinutes = 120,
        Status = status,
        RouteItems = new List<RouteItem>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Order = 1,
                EstimatedArrivalTime = new TimeOnly(8, 15),
                Status = "Planirano",
                Mailbox = MakeMailbox(43.85m, 18.41m)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Order = 2,
                EstimatedArrivalTime = new TimeOnly(8, 30),
                Status = "Planirano",
                Mailbox = MakeMailbox(43.86m, 18.42m)
            }
        }
    };

    [Fact]
    public async Task GetPostmanAssignedRouteForTodayAsync_ShouldReturnRoute_WhenAssignedRouteExists()
    {
        // Arrange
        var postmanId = Guid.NewGuid();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var route = MakeRoute(RouteStatus.Dodijeljena, postmanId, today);

        _routeRepositoryMock
            .Setup(repo => repo.GetByPostmanAndDateAsync(postmanId, today, It.IsAny<CancellationToken>()))
            .ReturnsAsync(route);

        // Act
        var result = await _sut.GetPostmanAssignedRouteForTodayAsync(postmanId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(route.Id);
        result.PostmanId.Should().Be(postmanId);
        result.Status.Should().Be(RouteStatus.Dodijeljena.ToString());
        result.RouteItems.Should().HaveCount(2);
        result.RouteItems[0].Order.Should().Be(1);
        result.RouteItems[1].Order.Should().Be(2);
    }

    [Fact]
    public async Task GetPostmanAssignedRouteForTodayAsync_ShouldReturnRoute_WhenRouteIsInProgress()
    {
        // Arrange
        var postmanId = Guid.NewGuid();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var route = MakeRoute(RouteStatus.UProgresu, postmanId, today);

        _routeRepositoryMock
            .Setup(repo => repo.GetByPostmanAndDateAsync(postmanId, today, It.IsAny<CancellationToken>()))
            .ReturnsAsync(route);

        // Act
        var result = await _sut.GetPostmanAssignedRouteForTodayAsync(postmanId);

        // Assert
        result.Should().NotBeNull();
        result!.Status.Should().Be(RouteStatus.UProgresu.ToString());
    }

    [Fact]
    public async Task GetPostmanAssignedRouteForTodayAsync_ShouldReturnNull_WhenNoRouteFound()
    {
        // Arrange
        var postmanId = Guid.NewGuid();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        _routeRepositoryMock
            .Setup(repo => repo.GetByPostmanAndDateAsync(postmanId, today, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Route?)null);

        // Act
        var result = await _sut.GetPostmanAssignedRouteForTodayAsync(postmanId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetPostmanAssignedRouteForTodayAsync_ShouldReturnNull_WhenRouteIsPlanned()
    {
        // Arrange
        var postmanId = Guid.NewGuid();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var route = MakeRoute(RouteStatus.Planirana, postmanId, today);

        _routeRepositoryMock
            .Setup(repo => repo.GetByPostmanAndDateAsync(postmanId, today, It.IsAny<CancellationToken>()))
            .ReturnsAsync(route);

        // Act
        var result = await _sut.GetPostmanAssignedRouteForTodayAsync(postmanId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetPostmanAssignedRouteForTodayAsync_ShouldReturnNull_WhenRouteCancelled()
    {
        // Arrange
        var postmanId = Guid.NewGuid();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var route = MakeRoute(RouteStatus.Otkazana, postmanId, today);

        _routeRepositoryMock
            .Setup(repo => repo.GetByPostmanAndDateAsync(postmanId, today, It.IsAny<CancellationToken>()))
            .ReturnsAsync(route);

        // Act
        var result = await _sut.GetPostmanAssignedRouteForTodayAsync(postmanId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetPostmanAssignedRouteForTodayAsync_ShouldReturnNull_WhenRouteCompleted()
    {
        // Arrange
        var postmanId = Guid.NewGuid();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var route = MakeRoute(RouteStatus.Zavrsena, postmanId, today);

        _routeRepositoryMock
            .Setup(repo => repo.GetByPostmanAndDateAsync(postmanId, today, It.IsAny<CancellationToken>()))
            .ReturnsAsync(route);

        // Act
        var result = await _sut.GetPostmanAssignedRouteForTodayAsync(postmanId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetPostmanAssignedRouteForTodayAsync_ShouldIncludeMailboxDetails_InRouteItems()
    {
        // Arrange
        var postmanId = Guid.NewGuid();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var mailbox1 = MakeMailbox(43.85m, 18.41m);
        var mailbox2 = MakeMailbox(43.86m, 18.42m);
        
        var route = new Route
        {
            Id = Guid.NewGuid(),
            PostmanId = postmanId,
            Date = today,
            PlannedStartTime = new TimeOnly(8, 0),
            PlannedEndTime = new TimeOnly(10, 0),
            TotalDistanceKm = 12.5m,
            TotalDurationMinutes = 120,
            Status = RouteStatus.Dodijeljena,
            RouteItems = new List<RouteItem>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    MailboxId = mailbox1.Id,
                    Order = 1,
                    EstimatedArrivalTime = new TimeOnly(8, 15),
                    Status = "Planirano",
                    Mailbox = mailbox1
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    MailboxId = mailbox2.Id,
                    Order = 2,
                    EstimatedArrivalTime = new TimeOnly(8, 30),
                    Status = "Planirano",
                    Mailbox = mailbox2
                }
            }
        };

        _routeRepositoryMock
            .Setup(repo => repo.GetByPostmanAndDateAsync(postmanId, today, It.IsAny<CancellationToken>()))
            .ReturnsAsync(route);

        // Act
        var result = await _sut.GetPostmanAssignedRouteForTodayAsync(postmanId);

        // Assert
        result.Should().NotBeNull();
        result!.RouteItems.Should().HaveCount(2);
        result.RouteItems[0].Address.Should().Be(mailbox1.Address);
        result.RouteItems[0].Latitude.Should().Be(mailbox1.Latitude);
        result.RouteItems[0].Longitude.Should().Be(mailbox1.Longitude);
        result.RouteItems[0].Priority.Should().Be(mailbox1.Priority.ToString());
        result.RouteItems[1].Address.Should().Be(mailbox2.Address);
    }
}
