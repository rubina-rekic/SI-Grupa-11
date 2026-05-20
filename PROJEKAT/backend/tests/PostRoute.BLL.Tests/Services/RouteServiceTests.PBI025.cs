using FluentAssertions;
using Moq;
using PostRoute.BLL.Models.Routes;
using PostRoute.BLL.Services;
using PostRoute.DAL.Entities;
using PostRoute.DAL.Repositories;

namespace PostRoute.BLL.Tests.Services;

public class RouteServiceTestsPBI025
{
    private readonly Mock<IMailboxRepository> _mailboxRepositoryMock;
    private readonly Mock<IRouteRepository> _routeRepositoryMock;
    private readonly RouteService _sut;

    public RouteServiceTestsPBI025()
    {
        _mailboxRepositoryMock = new Mock<IMailboxRepository>();
        _routeRepositoryMock = new Mock<IRouteRepository>();
        _sut = new RouteService(_mailboxRepositoryMock.Object, _routeRepositoryMock.Object);

        _routeRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Route>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Route r, CancellationToken _) => r);
    }

    private static Mailbox MakeMailbox(decimal lat = 43.85m, decimal lng = 18.41m) => new()
    {
        Id = Guid.NewGuid(),
        Address = "Test adresa",
        Latitude = lat,
        Longitude = lng,
        Priority = MailboxPriority.Visok,
        IsAlwaysAvailable = true,
        IsActive = true
    };

    private static Route MakeRoute(RouteStatus status, IList<RouteItem> items) => new()
    {
        Id = Guid.NewGuid(),
        PostmanId = Guid.NewGuid(),
        Date = new DateOnly(2026, 5, 20),
        PlannedStartTime = new TimeOnly(8, 0),
        Status = status,
        RouteItems = items
    };

    [Fact]
    public async Task ReorderRouteAsync_ShouldThrow_WhenRouteNotFound()
    {
        _routeRepositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Route?)null);

        var act = () => _sut.ReorderRouteAsync(Guid.NewGuid(), new ReorderRouteRequest(), "dispatcher");

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Ruta nije pronađena.");
    }

    [Fact]
    public async Task ReorderRouteAsync_ShouldThrow_WhenRouteIsInProgress()
    {
        var route = MakeRoute(RouteStatus.UProgresu, new List<RouteItem>());
        _routeRepositoryMock
            .Setup(r => r.GetByIdAsync(route.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(route);

        var act = () => _sut.ReorderRouteAsync(route.Id, new ReorderRouteRequest(), "dispatcher");

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ReorderRouteAsync_ShouldThrow_WhenRouteIsFinished()
    {
        var route = MakeRoute(RouteStatus.Zavrsena, new List<RouteItem>());
        _routeRepositoryMock
            .Setup(r => r.GetByIdAsync(route.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(route);

        var act = () => _sut.ReorderRouteAsync(route.Id, new ReorderRouteRequest(), "dispatcher");

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ReorderRouteAsync_ShouldApplyNewOrder_WhenRouteIsPlanirana()
    {
        var mb1 = MakeMailbox(43.85m, 18.41m);
        var mb2 = MakeMailbox(43.86m, 18.42m);
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();

        var route = MakeRoute(RouteStatus.Planirana, new List<RouteItem>
        {
            new() { Id = id1, Order = 1, MailboxId = mb1.Id, Mailbox = mb1, EstimatedArrivalTime = new TimeOnly(8, 10), Status = "Planirano" },
            new() { Id = id2, Order = 2, MailboxId = mb2.Id, Mailbox = mb2, EstimatedArrivalTime = new TimeOnly(8, 20), Status = "Planirano" }
        });

        _routeRepositoryMock
            .Setup(r => r.GetByIdAsync(route.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(route);

        var request = new ReorderRouteRequest
        {
            Items = new List<ReorderItem>
            {
                new() { RouteItemId = id1, NewOrder = 2 },
                new() { RouteItemId = id2, NewOrder = 1 }
            }
        };

        var result = await _sut.ReorderRouteAsync(route.Id, request, "dispatcher");

        result.RouteItems.Single(x => x.Id == id1).Order.Should().Be(2);
        result.RouteItems.Single(x => x.Id == id2).Order.Should().Be(1);
    }

    [Fact]
    public async Task ReorderRouteAsync_ShouldMarkMovedItems_AsManuallyReordered()
    {
        var mb1 = MakeMailbox(43.85m, 18.41m);
        var mb2 = MakeMailbox(43.86m, 18.42m);
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();

        var route = MakeRoute(RouteStatus.Planirana, new List<RouteItem>
        {
            new() { Id = id1, Order = 1, MailboxId = mb1.Id, Mailbox = mb1, EstimatedArrivalTime = new TimeOnly(8, 10), Status = "Planirano", IsManuallyReordered = false },
            new() { Id = id2, Order = 2, MailboxId = mb2.Id, Mailbox = mb2, EstimatedArrivalTime = new TimeOnly(8, 20), Status = "Planirano", IsManuallyReordered = false }
        });

        _routeRepositoryMock
            .Setup(r => r.GetByIdAsync(route.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(route);

        var request = new ReorderRouteRequest
        {
            Items = new List<ReorderItem>
            {
                new() { RouteItemId = id1, NewOrder = 2 },
                new() { RouteItemId = id2, NewOrder = 1 }
            }
        };

        var result = await _sut.ReorderRouteAsync(route.Id, request, "dispatcher");

        result.RouteItems.Should().AllSatisfy(x => x.IsManuallyReordered.Should().BeTrue());
    }

    [Fact]
    public async Task ReorderRouteAsync_ShouldNotMarkItems_AsManuallyReordered_WhenOrderUnchanged()
    {
        var mb1 = MakeMailbox(43.85m, 18.41m);
        var mb2 = MakeMailbox(43.86m, 18.42m);
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();

        var route = MakeRoute(RouteStatus.Planirana, new List<RouteItem>
        {
            new() { Id = id1, Order = 1, MailboxId = mb1.Id, Mailbox = mb1, EstimatedArrivalTime = new TimeOnly(8, 10), Status = "Planirano", IsManuallyReordered = false },
            new() { Id = id2, Order = 2, MailboxId = mb2.Id, Mailbox = mb2, EstimatedArrivalTime = new TimeOnly(8, 20), Status = "Planirano", IsManuallyReordered = false }
        });

        _routeRepositoryMock
            .Setup(r => r.GetByIdAsync(route.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(route);

        var request = new ReorderRouteRequest
        {
            Items = new List<ReorderItem>
            {
                new() { RouteItemId = id1, NewOrder = 1 },
                new() { RouteItemId = id2, NewOrder = 2 }
            }
        };

        var result = await _sut.ReorderRouteAsync(route.Id, request, "dispatcher");

        result.RouteItems.Should().AllSatisfy(x => x.IsManuallyReordered.Should().BeFalse());
    }

    [Fact]
    public async Task ReorderRouteAsync_ShouldRecalculateArrivalTimes_AfterReorder()
    {
        // mb1 je blizu depoa, mb2 je daleko — originalni redoslijed: mb1→mb2
        // nakon swapa: mb2→mb1, pa mb1 (sada drugi) ima drugačije vrijeme dolaska
        var mb1 = MakeMailbox(43.85m, 18.41m);
        var mb2 = MakeMailbox(43.90m, 18.50m);
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var originalArrival1 = new TimeOnly(8, 10);

        var route = MakeRoute(RouteStatus.Planirana, new List<RouteItem>
        {
            new() { Id = id1, Order = 1, MailboxId = mb1.Id, Mailbox = mb1, EstimatedArrivalTime = originalArrival1, Status = "Planirano" },
            new() { Id = id2, Order = 2, MailboxId = mb2.Id, Mailbox = mb2, EstimatedArrivalTime = new TimeOnly(8, 30), Status = "Planirano" }
        });

        _routeRepositoryMock
            .Setup(r => r.GetByIdAsync(route.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(route);

        var request = new ReorderRouteRequest
        {
            Items = new List<ReorderItem>
            {
                new() { RouteItemId = id1, NewOrder = 2 },
                new() { RouteItemId = id2, NewOrder = 1 }
            }
        };

        var result = await _sut.ReorderRouteAsync(route.Id, request, "dispatcher");

        // Stavka 1 je sada na poziciji 2 — dolazi nakon mb2 + stop → različito od originalnog
        result.RouteItems.Single(x => x.Id == id1).EstimatedArrivalTime
            .Should().NotBe(originalArrival1);
    }

    [Fact]
    public async Task ReorderRouteAsync_ShouldUpdateTotalDuration_AfterReorder()
    {
        var mb1 = MakeMailbox(43.85m, 18.41m);
        var mb2 = MakeMailbox(43.90m, 18.50m);
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();

        var route = MakeRoute(RouteStatus.Planirana, new List<RouteItem>
        {
            new() { Id = id1, Order = 1, MailboxId = mb1.Id, Mailbox = mb1, EstimatedArrivalTime = new TimeOnly(8, 10), Status = "Planirano" },
            new() { Id = id2, Order = 2, MailboxId = mb2.Id, Mailbox = mb2, EstimatedArrivalTime = new TimeOnly(8, 30), Status = "Planirano" }
        });

        _routeRepositoryMock
            .Setup(r => r.GetByIdAsync(route.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(route);

        var request = new ReorderRouteRequest
        {
            Items = new List<ReorderItem>
            {
                new() { RouteItemId = id1, NewOrder = 2 },
                new() { RouteItemId = id2, NewOrder = 1 }
            }
        };

        var result = await _sut.ReorderRouteAsync(route.Id, request, "dispatcher");

        result.TotalDurationMinutes.Should().BeGreaterThan(0);
        result.PlannedEndTime.Should().NotBeNull();
    }

    [Fact]
    public async Task ReorderRouteAsync_ShouldSetAuditFields_WithDispatcherNameAndTimestamp()
    {
        var mb1 = MakeMailbox();
        var id1 = Guid.NewGuid();

        var route = MakeRoute(RouteStatus.Planirana, new List<RouteItem>
        {
            new() { Id = id1, Order = 1, MailboxId = mb1.Id, Mailbox = mb1, EstimatedArrivalTime = new TimeOnly(8, 10), Status = "Planirano" }
        });

        _routeRepositoryMock
            .Setup(r => r.GetByIdAsync(route.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(route);

        var request = new ReorderRouteRequest
        {
            Items = new List<ReorderItem> { new() { RouteItemId = id1, NewOrder = 1 } }
        };

        await _sut.ReorderRouteAsync(route.Id, request, "kerim");

        route.LastReorderedBy.Should().Be("kerim");
        route.LastReorderedAt.Should().NotBeNull();
        route.LastReorderedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task ReorderRouteAsync_ShouldCallUpdateAsync_ExactlyOnce()
    {
        var mb1 = MakeMailbox();
        var id1 = Guid.NewGuid();

        var route = MakeRoute(RouteStatus.Planirana, new List<RouteItem>
        {
            new() { Id = id1, Order = 1, MailboxId = mb1.Id, Mailbox = mb1, EstimatedArrivalTime = new TimeOnly(8, 10), Status = "Planirano" }
        });

        _routeRepositoryMock
            .Setup(r => r.GetByIdAsync(route.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(route);

        var request = new ReorderRouteRequest
        {
            Items = new List<ReorderItem> { new() { RouteItemId = id1, NewOrder = 1 } }
        };

        await _sut.ReorderRouteAsync(route.Id, request, "dispatcher");

        _routeRepositoryMock.Verify(
            r => r.UpdateAsync(It.IsAny<Route>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ReorderRouteAsync_ShouldAllowReorder_WhenRouteIsOtkazana()
    {
        // Otkazana nije UProgresu ni Zavrsena, pa ne smije baciti exception
        var mb1 = MakeMailbox();
        var id1 = Guid.NewGuid();

        var route = MakeRoute(RouteStatus.Otkazana, new List<RouteItem>
        {
            new() { Id = id1, Order = 1, MailboxId = mb1.Id, Mailbox = mb1, EstimatedArrivalTime = new TimeOnly(8, 10), Status = "Planirano" }
        });

        _routeRepositoryMock
            .Setup(r => r.GetByIdAsync(route.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(route);

        var request = new ReorderRouteRequest
        {
            Items = new List<ReorderItem> { new() { RouteItemId = id1, NewOrder = 1 } }
        };

        var act = () => _sut.ReorderRouteAsync(route.Id, request, "dispatcher");

        await act.Should().NotThrowAsync();
    }
}
