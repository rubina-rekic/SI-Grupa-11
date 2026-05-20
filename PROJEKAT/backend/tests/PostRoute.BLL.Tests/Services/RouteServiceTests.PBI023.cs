using FluentAssertions;
using Moq;
using PostRoute.BLL.Models.Routes;
using PostRoute.BLL.Services;
using PostRoute.DAL.Entities;
using PostRoute.DAL.Repositories;
using PostRoute.Domain.Entities;

namespace PostRoute.BLL.Tests.Services;

public class RouteServiceTestsPBI023
{
    private readonly Mock<IMailboxRepository> _mailboxRepositoryMock = new();
    private readonly Mock<IRouteRepository> _routeRepositoryMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly RouteService _sut;

    public RouteServiceTestsPBI023()
    {
        _sut = new RouteService(
            _mailboxRepositoryMock.Object,
            _routeRepositoryMock.Object,
            _userRepositoryMock.Object);

        _routeRepositoryMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<Route>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Route route, CancellationToken _) => route);
    }

    private static User MakePostman(string firstName = "Amar", string lastName = "Hodzic") => new()
    {
        Id = Guid.NewGuid(),
        FirstName = firstName,
        LastName = lastName,
        Username = $"{firstName}.{lastName}".ToLowerInvariant(),
        Email = $"{firstName}.{lastName}@postroute.ba".ToLowerInvariant(),
        Role = UserRole.PostalWorker,
        IsLockedOut = false
    };

    private static Route MakeRoute(RouteStatus status, Guid? postmanId = null) => new()
    {
        Id = Guid.NewGuid(),
        PostmanId = postmanId ?? Guid.NewGuid(),
        Date = new DateOnly(2026, 5, 20),
        PlannedStartTime = new TimeOnly(8, 0),
        PlannedEndTime = new TimeOnly(10, 0),
        TotalDistanceKm = 12.5m,
        TotalDurationMinutes = 120,
        Status = status,
        RouteItems = new List<RouteItem>()
    };

    [Fact]
    public async Task AssignRouteAsync_ShouldAssignPlaniranaRoute_ToAvailablePostman()
    {
        var route = MakeRoute(RouteStatus.Planirana);
        var postman = MakePostman();

        _routeRepositoryMock
            .Setup(repo => repo.GetByIdAsync(route.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(route);
        _routeRepositoryMock
            .Setup(repo => repo.GetPostmanIdsWithActiveRouteOnDateAsync(route.Date, route.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Guid>());
        _userRepositoryMock
            .Setup(repo => repo.GetByIdAsync(postman.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(postman);

        var result = await _sut.AssignRouteAsync(
            route.Id,
            new AssignRouteRequest { PostmanId = postman.Id },
            "dispatcher");

        result.PostmanId.Should().Be(postman.Id);
        result.PostmanName.Should().Be("Amar Hodzic");
        result.Status.Should().Be(RouteStatus.Dodijeljena.ToString());
        result.AssignedBy.Should().Be("dispatcher");
        result.AssignedAt.Should().NotBeNull();
        route.Status.Should().Be(RouteStatus.Dodijeljena);
        route.AssignedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        _routeRepositoryMock.Verify(repo => repo.UpdateAsync(route, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AssignRouteAsync_ShouldThrow_WhenPostmanAlreadyHasActiveRouteForDate()
    {
        var route = MakeRoute(RouteStatus.Planirana);
        var postman = MakePostman();

        _routeRepositoryMock
            .Setup(repo => repo.GetByIdAsync(route.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(route);
        _routeRepositoryMock
            .Setup(repo => repo.GetPostmanIdsWithActiveRouteOnDateAsync(route.Date, route.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { postman.Id });
        _userRepositoryMock
            .Setup(repo => repo.GetByIdAsync(postman.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(postman);

        var act = () => _sut.AssignRouteAsync(
            route.Id,
            new AssignRouteRequest { PostmanId = postman.Id },
            "dispatcher");

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Postar vec ima dodijeljenu rutu za ovaj datum.");
        _routeRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Route>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AssignRouteAsync_ShouldThrow_WhenRouteIsInProgress()
    {
        var route = MakeRoute(RouteStatus.UProgresu);

        _routeRepositoryMock
            .Setup(repo => repo.GetByIdAsync(route.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(route);

        var act = () => _sut.AssignRouteAsync(
            route.Id,
            new AssignRouteRequest { PostmanId = Guid.NewGuid() },
            "dispatcher");

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Dodjela rute je dostupna samo za prijedloge ili vec dodijeljene rute.");
    }

    [Fact]
    public async Task AssignRouteAsync_ShouldThrow_WhenUserIsNotActivePostman()
    {
        var route = MakeRoute(RouteStatus.Planirana);
        var lockedPostman = MakePostman();
        lockedPostman.IsLockedOut = true;

        _routeRepositoryMock
            .Setup(repo => repo.GetByIdAsync(route.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(route);
        _userRepositoryMock
            .Setup(repo => repo.GetByIdAsync(lockedPostman.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(lockedPostman);

        var act = () => _sut.AssignRouteAsync(
            route.Id,
            new AssignRouteRequest { PostmanId = lockedPostman.Id },
            "dispatcher");

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Odabrani korisnik nije aktivan postar.");
    }

    [Fact]
    public async Task GetAvailablePostmenAsync_ShouldReturnActivePostmenWithAvailability()
    {
        var available = MakePostman("Lejla", "Basic");
        var busy = MakePostman("Tarik", "Music");
        var locked = MakePostman("Sara", "Locked");
        locked.IsLockedOut = true;
        var dispatcher = new User
        {
            Id = Guid.NewGuid(),
            FirstName = "Dina",
            LastName = "Dispatcher",
            Username = "dina.dispatcher",
            Email = "dina@postroute.ba",
            Role = UserRole.Dispatcher
        };
        var route = MakeRoute(RouteStatus.Planirana);

        _routeRepositoryMock
            .Setup(repo => repo.GetByIdAsync(route.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(route);
        _routeRepositoryMock
            .Setup(repo => repo.GetPostmanIdsWithActiveRouteOnDateAsync(route.Date, route.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { busy.Id });
        _userRepositoryMock
            .Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { available, busy, locked, dispatcher });

        var result = await _sut.GetAvailablePostmenAsync(route.Id);

        result.Should().HaveCount(2);
        result.Single(x => x.Id == available.Id).IsAvailable.Should().BeTrue();
        result.Single(x => x.Id == busy.Id).IsAvailable.Should().BeFalse();
        result.Single(x => x.Id == busy.Id).UnavailableReason
            .Should().Be("Postar vec ima dodijeljenu rutu za ovaj datum.");
        result.Should().NotContain(x => x.Id == locked.Id);
        result.Should().NotContain(x => x.Id == dispatcher.Id);
    }
}
