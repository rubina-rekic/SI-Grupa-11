using FluentAssertions;
using Moq;
using PostRoute.BLL.Models.Routes;
using PostRoute.BLL.Services;
using PostRoute.DAL.Entities;
using PostRoute.DAL.Repositories;

namespace PostRoute.BLL.Tests;

public class RouteServiceTestsPBI022
{
    private readonly Mock<IMailboxRepository> _mailboxRepositoryMock;
    private readonly Mock<IRouteRepository> _routeRepositoryMock;
    private readonly RouteService _sut;

    public RouteServiceTestsPBI022()
    {
        _mailboxRepositoryMock = new Mock<IMailboxRepository>();
        _routeRepositoryMock = new Mock<IRouteRepository>();
        _sut = new RouteService(_mailboxRepositoryMock.Object, _routeRepositoryMock.Object);

        _routeRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Route>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Route r, CancellationToken _) => r);
        _routeRepositoryMock.Setup(repo => repo.GetByPostmanAndDateAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Route?)null);
        _routeRepositoryMock.Setup(repo => repo.GetLastIncludedDatesByMailboxIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Guid, DateOnly>());
    }

    [Fact]
    public async Task GenerateRouteAsync_ShouldReturnExistingRoute_WhenSamePostmanAndDateAlreadyExist()
    {
        var mailbox = new Mailbox { Id = Guid.NewGuid(), Address = "A", Latitude = 43.85m, Longitude = 18.41m, Priority = MailboxPriority.Visok };
        var existing = new Route
        {
            Id = Guid.NewGuid(),
            PostmanId = Guid.NewGuid(),
            Date = new DateOnly(2026, 5, 12),
            PlannedStartTime = new TimeOnly(8, 0),
            PlannedEndTime = new TimeOnly(8, 30),
            TotalDistanceKm = 3.4m,
            TotalDurationMinutes = 30,
            RouteItems = new List<RouteItem>
            {
                new() { Id = Guid.NewGuid(), Order = 1, MailboxId = mailbox.Id, Mailbox = mailbox, EstimatedArrivalTime = new TimeOnly(8, 10), Status = "Planirano" }
            }
        };

        _routeRepositoryMock.Setup(repo => repo.GetByPostmanAndDateAsync(existing.PostmanId, existing.Date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var request = new GenerateRouteRequest { PostmanId = existing.PostmanId, Date = existing.Date, PlannedStartTime = new TimeOnly(8, 0) };

        var result = await _sut.GenerateRouteAsync(request);

        result.Id.Should().Be(existing.Id);
        result.RouteItems.Should().HaveCount(1);
        _routeRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Route>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GenerateRouteAsync_ShouldFilterOutInactiveMailboxes()
    {
        _mailboxRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Mailbox>
            {
                new() { Id = Guid.NewGuid(), IsActive = false, IsAlwaysAvailable = true, Latitude = 43.85m, Longitude = 18.41m, Priority = MailboxPriority.Visok }
            });

        var request = new GenerateRouteRequest { Date = DateOnly.FromDateTime(DateTime.UtcNow), PlannedStartTime = new TimeOnly(8, 0) };

        var result = await _sut.GenerateRouteAsync(request);

        result.RouteItems.Should().BeEmpty();
    }

    [Fact]
    public async Task GenerateRouteAsync_ShouldExcludeMailboxesOutsideTimeWindow()
    {
        var monday = new DateOnly(2026, 5, 19);

        _mailboxRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Mailbox>
        {
            new()
            {
                Id = Guid.NewGuid(), IsActive = true, IsAlwaysAvailable = false,
                WorkingDays = MailboxWorkingDays.SvakiDan,
                Slot1Start = new TimeOnly(12, 0), Slot1End = new TimeOnly(14, 0),
                Latitude = 43.85m, Longitude = 18.41m, Priority = MailboxPriority.Visok
            }
        });

        var request = new GenerateRouteRequest { Date = monday, PlannedStartTime = new TimeOnly(8, 0) };

        var result = await _sut.GenerateRouteAsync(request);

        result.RouteItems.Should().BeEmpty();
    }

    [Fact]
    public async Task GenerateRouteAsync_ShouldExcludeMailboxes_WhenNotWorkingOnRouteDay()
    {
        var saturday = new DateOnly(2026, 5, 17);

        _mailboxRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Mailbox>
        {
            new()
            {
                Id = Guid.NewGuid(), IsActive = true, IsAlwaysAvailable = true,
                WorkingDays = MailboxWorkingDays.RadniDani,
                Latitude = 43.85m, Longitude = 18.41m, Priority = MailboxPriority.Visok
            }
        });

        var request = new GenerateRouteRequest { Date = saturday, PlannedStartTime = new TimeOnly(8, 0) };

        var result = await _sut.GenerateRouteAsync(request);

        result.RouteItems.Should().BeEmpty();
    }

    [Fact]
    public async Task GenerateRouteAsync_ShouldIncludeMailbox_WhenWorkingOnRouteDay()
    {
        var saturday = new DateOnly(2026, 5, 17);

        var mailbox = new Mailbox
        {
            Id = Guid.NewGuid(), IsActive = true, IsAlwaysAvailable = true,
            WorkingDays = MailboxWorkingDays.SvakiDan,
            Latitude = 43.85m, Longitude = 18.41m, Priority = MailboxPriority.Visok
        };

        _mailboxRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Mailbox> { mailbox });

        var request = new GenerateRouteRequest { Date = saturday, PlannedStartTime = new TimeOnly(8, 0) };

        var result = await _sut.GenerateRouteAsync(request);

        result.RouteItems.Should().HaveCount(1);
        result.RouteItems[0].MailboxId.Should().Be(mailbox.Id);
    }

    [Fact]
    public async Task GenerateRouteAsync_ShouldApplyPriorityCooldownRules()
    {
        var date = new DateOnly(2026, 5, 12);
        var high = new Mailbox { Id = Guid.NewGuid(), IsActive = true, IsAlwaysAvailable = true, Latitude = 43.8560m, Longitude = 18.4130m, Priority = MailboxPriority.Visok };
        var mediumDue = new Mailbox { Id = Guid.NewGuid(), IsActive = true, IsAlwaysAvailable = true, Latitude = 43.8570m, Longitude = 18.4130m, Priority = MailboxPriority.Srednji };
        var lowTooSoon = new Mailbox { Id = Guid.NewGuid(), IsActive = true, IsAlwaysAvailable = true, Latitude = 43.8580m, Longitude = 18.4130m, Priority = MailboxPriority.Nizak };
        var lowDue = new Mailbox { Id = Guid.NewGuid(), IsActive = true, IsAlwaysAvailable = true, Latitude = 43.8590m, Longitude = 18.4130m, Priority = MailboxPriority.Nizak };

        _mailboxRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Mailbox> { high, mediumDue, lowTooSoon, lowDue });

        _routeRepositoryMock.Setup(repo => repo.GetLastIncludedDatesByMailboxIdsAsync(It.IsAny<IEnumerable<Guid>>(), date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Guid, DateOnly>
            {
                [high.Id] = date.AddDays(-1),
                [mediumDue.Id] = date.AddDays(-2),
                [lowTooSoon.Id] = date.AddDays(-3),
                [lowDue.Id] = date.AddDays(-4)
            });

        var request = new GenerateRouteRequest { Date = date, PlannedStartTime = new TimeOnly(8, 0) };

        var result = await _sut.GenerateRouteAsync(request);

        result.RouteItems.Select(x => x.MailboxId).Should().Contain(high.Id);
        result.RouteItems.Select(x => x.MailboxId).Should().Contain(mediumDue.Id);
        result.RouteItems.Select(x => x.MailboxId).Should().Contain(lowDue.Id);
        result.RouteItems.Select(x => x.MailboxId).Should().NotContain(lowTooSoon.Id);
    }
}
