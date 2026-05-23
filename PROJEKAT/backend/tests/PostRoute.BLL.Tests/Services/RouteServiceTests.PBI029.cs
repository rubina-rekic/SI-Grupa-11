using FluentAssertions;
using Moq;
using PostRoute.BLL.Services;
using PostRoute.DAL.Entities;
using PostRoute.DAL.Repositories;
using PostRoute.Domain.Entities;

namespace PostRoute.BLL.Tests.Services;

public class RouteServiceTestsPBI029
{
    private readonly Mock<IMailboxRepository> _mailboxRepositoryMock = new();
    private readonly Mock<IRouteRepository> _routeRepositoryMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly RouteService _sut;

    public RouteServiceTestsPBI029()
    {
        _sut = new RouteService(
            _mailboxRepositoryMock.Object,
            _routeRepositoryMock.Object,
            _userRepositoryMock.Object);
    }

    private static User MakePostman(string firstName = "Amar", string lastName = "Hodžić") => new()
    {
        Id = Guid.NewGuid(),
        FirstName = firstName,
        LastName = lastName,
        Username = "amar.hodzic",
        Email = "amar@test.ba",
        PasswordHash = "hash",
        Role = "PostalWorker"
    };

    private static Mailbox MakeMailbox() => new()
    {
        Id = Guid.NewGuid(),
        SerialNumber = "SN001",
        Address = "Test adresa",
        Latitude = 43.85m,
        Longitude = 18.41m,
        Priority = MailboxPriority.Srednji,
        IsAlwaysAvailable = true,
        IsActive = true,
        Type = MailboxType.StandaloneLarge,
        Status = MailboxStatus.Prazan,
        Capacity = 100,
        InstallationYear = 2020
    };

    private static Route MakeRoute(RouteStatus status, User? postman = null, DateOnly? date = null) => new()
    {
        Id = Guid.NewGuid(),
        PostmanId = postman?.Id ?? Guid.NewGuid(),
        Postman = postman,
        Date = date ?? new DateOnly(2026, 5, 23),
        PlannedStartTime = new TimeOnly(8, 0),
        PlannedEndTime = new TimeOnly(10, 0),
        TotalDistanceKm = 10m,
        TotalDurationMinutes = 90,
        Status = status,
        RouteItems = new List<RouteItem>()
    };

    // ================================================================
    // VRAĆA RUTE ZA DATUM
    // ================================================================

    [Fact]
    public async Task GetRoutesForDateAsync_ShouldReturnAllRoutes_ForGivenDate()
    {
        var date = new DateOnly(2026, 5, 23);
        var routes = new List<Route>
        {
            MakeRoute(RouteStatus.Dodijeljena, date: date),
            MakeRoute(RouteStatus.UProgresu,   date: date),
            MakeRoute(RouteStatus.Planirana,   date: date),
        };

        _routeRepositoryMock
            .Setup(r => r.GetByDateAsync(date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(routes);

        var result = await _sut.GetRoutesForDateAsync(date);

        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetRoutesForDateAsync_ShouldReturnEmptyList_WhenNoRoutesForDate()
    {
        var date = new DateOnly(2026, 5, 23);

        _routeRepositoryMock
            .Setup(r => r.GetByDateAsync(date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Route>());

        var result = await _sut.GetRoutesForDateAsync(date);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetRoutesForDateAsync_ShouldPassCorrectDate_ToRepository()
    {
        var date = new DateOnly(2026, 5, 23);

        _routeRepositoryMock
            .Setup(r => r.GetByDateAsync(It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Route>());

        await _sut.GetRoutesForDateAsync(date);

        _routeRepositoryMock.Verify(
            r => r.GetByDateAsync(date, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    // ================================================================
    // MAPIRANJE
    // ================================================================

    [Fact]
    public async Task GetRoutesForDateAsync_ShouldMapPostmanName_WhenPostmanExists()
    {
        var date = new DateOnly(2026, 5, 23);
        var postman = MakePostman("Amar", "Hodžić");
        var route = MakeRoute(RouteStatus.Dodijeljena, postman, date);

        _routeRepositoryMock
            .Setup(r => r.GetByDateAsync(date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Route> { route });

        var result = await _sut.GetRoutesForDateAsync(date);

        result[0].PostmanName.Should().Be("Amar Hodžić");
        result[0].PostmanId.Should().Be(postman.Id);
    }

    [Fact]
    public async Task GetRoutesForDateAsync_ShouldMapStatus_ToStringRepresentation()
    {
        var date = new DateOnly(2026, 5, 23);
        var routes = new List<Route>
        {
            MakeRoute(RouteStatus.UProgresu,   date: date),
            MakeRoute(RouteStatus.Dodijeljena, date: date),
            MakeRoute(RouteStatus.Zavrsena,    date: date),
        };

        _routeRepositoryMock
            .Setup(r => r.GetByDateAsync(date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(routes);

        var result = await _sut.GetRoutesForDateAsync(date);

        result.Select(r => r.Status).Should().BeEquivalentTo(
            new[] { "UProgresu", "Dodijeljena", "Zavrsena" });
    }

    [Fact]
    public async Task GetRoutesForDateAsync_ShouldIncludeRouteItems_WithMailboxStatus()
    {
        var date = new DateOnly(2026, 5, 23);
        var mailbox = MakeMailbox();
        mailbox.Status = MailboxStatus.Napunjen;

        var route = MakeRoute(RouteStatus.UProgresu, date: date);
        route.RouteItems = new List<RouteItem>
        {
            new()
            {
                Id = Guid.NewGuid(),
                MailboxId = mailbox.Id,
                Mailbox = mailbox,
                Order = 1,
                EstimatedArrivalTime = new TimeOnly(8, 15),
                Status = "Planirano"
            }
        };

        _routeRepositoryMock
            .Setup(r => r.GetByDateAsync(date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Route> { route });

        var result = await _sut.GetRoutesForDateAsync(date);

        result[0].RouteItems.Should().HaveCount(1);
        result[0].RouteItems[0].MailboxStatus.Should().Be("Napunjen");
        result[0].RouteItems[0].Address.Should().Be(mailbox.Address);
    }

    [Fact]
    public async Task GetRoutesForDateAsync_ShouldReturnRouteWithId_MatchingOriginalRoute()
    {
        var date = new DateOnly(2026, 5, 23);
        var route = MakeRoute(RouteStatus.Dodijeljena, date: date);

        _routeRepositoryMock
            .Setup(r => r.GetByDateAsync(date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Route> { route });

        var result = await _sut.GetRoutesForDateAsync(date);

        result[0].Id.Should().Be(route.Id);
        result[0].Date.Should().Be(date);
    }
}
