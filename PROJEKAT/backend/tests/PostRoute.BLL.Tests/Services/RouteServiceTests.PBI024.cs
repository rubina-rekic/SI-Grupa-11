using FluentAssertions;
using Moq;
using PostRoute.BLL.Models.Routes;
using PostRoute.BLL.Services;
using PostRoute.DAL.Entities;
using PostRoute.DAL.Repositories;
using Xunit;

namespace PostRoute.BLL.Tests.Services;

public class RouteServiceTestsPBI024
{
    private readonly Mock<IMailboxRepository> _mailboxRepositoryMock;
    private readonly Mock<IRouteRepository> _routeRepositoryMock;
    private readonly RouteService _sut;

    public RouteServiceTestsPBI024()
    {
        _mailboxRepositoryMock = new Mock<IMailboxRepository>();
        _routeRepositoryMock = new Mock<IRouteRepository>();
        _sut = new RouteService(_mailboxRepositoryMock.Object, _routeRepositoryMock.Object);
    }

    [Fact]
    public async Task GetRouteDetailsAsync_ShouldReturnNull_WhenRouteNotFound()
    {
        // Arrange
        var routeId = Guid.NewGuid();
        _routeRepositoryMock.Setup(repo => repo.GetByIdAsync(routeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Route?)null);

        // Act
        var result = await _sut.GetRouteDetailsAsync(routeId);

        // Assert
        result.Should().BeNull();
        _routeRepositoryMock.Verify(repo => repo.GetByIdAsync(routeId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetRouteDetailsAsync_ShouldReturnRouteDetails_WhenRouteExists()
    {
        // Arrange
        var routeId = Guid.NewGuid();
        var postmanId = Guid.NewGuid();
        var mailboxId = Guid.NewGuid();

        var mailbox = new Mailbox
        {
            Id = mailboxId,
            SerialNumber = "SN001",
            Address = "Sarajevo, Test 1",
            Latitude = 43.85m,
            Longitude = 18.41m,
            Type = MailboxType.WallSmall,
            Priority = MailboxPriority.Visok,
            Status = MailboxStatus.Prazan,
            Capacity = 100,
            InstallationYear = 2020
        };

        var route = new Route
        {
            Id = routeId,
            PostmanId = postmanId,
            Date = new DateOnly(2026, 5, 19),
            PlannedStartTime = new TimeOnly(8, 0),
            PlannedEndTime = new TimeOnly(12, 0),
            TotalDistanceKm = 15.5m,
            TotalDurationMinutes = 240,
            RouteItems = new List<RouteItem>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Order = 1,
                    MailboxId = mailboxId,
                    Mailbox = mailbox,
                    EstimatedArrivalTime = new TimeOnly(8, 15),
                    Status = "Planirano"
                }
            }
        };

        _routeRepositoryMock.Setup(repo => repo.GetByIdAsync(routeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(route);

        // Act
        var result = await _sut.GetRouteDetailsAsync(routeId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(routeId);
        result.PostmanId.Should().Be(postmanId);
        result.Date.Should().Be(new DateOnly(2026, 5, 19));
        result.PlannedStartTime.Should().Be(new TimeOnly(8, 0));
        result.PlannedEndTime.Should().Be(new TimeOnly(12, 0));
        result.TotalDistanceKm.Should().Be(15.5m);
        result.TotalDurationMinutes.Should().Be(240);
    }

    [Fact]
    public async Task GetRouteDetailsAsync_ShouldIncludeRouteItems_InResponse()
    {
        // Arrange
        var routeId = Guid.NewGuid();
        var postmanId = Guid.NewGuid();
        var mailboxId = Guid.NewGuid();

        var mailbox = new Mailbox
        {
            Id = mailboxId,
            SerialNumber = "SN001",
            Address = "Sarajevo, Test 1",
            Latitude = 43.85m,
            Longitude = 18.41m,
            Type = MailboxType.WallSmall,
            Priority = MailboxPriority.Visok,
            Status = MailboxStatus.Prazan,
            Capacity = 100,
            InstallationYear = 2020
        };

        var route = new Route
        {
            Id = routeId,
            PostmanId = postmanId,
            Date = new DateOnly(2026, 5, 19),
            PlannedStartTime = new TimeOnly(8, 0),
            PlannedEndTime = new TimeOnly(12, 0),
            TotalDistanceKm = 15.5m,
            TotalDurationMinutes = 240,
            RouteItems = new List<RouteItem>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Order = 1,
                    MailboxId = mailboxId,
                    Mailbox = mailbox,
                    EstimatedArrivalTime = new TimeOnly(8, 15),
                    Status = "Planirano"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Order = 2,
                    MailboxId = Guid.NewGuid(),
                    Mailbox = new Mailbox
                    {
                        Id = Guid.NewGuid(),
                        SerialNumber = "SN002",
                        Address = "Sarajevo, Test 2",
                        Latitude = 43.86m,
                        Longitude = 18.42m,
                        Type = MailboxType.StandaloneLarge,
                        Priority = MailboxPriority.Srednji,
                        Status = MailboxStatus.Pun,
                        Capacity = 200,
                        InstallationYear = 2021
                    },
                    EstimatedArrivalTime = new TimeOnly(8, 45),
                    Status = "Planirano"
                }
            }
        };

        _routeRepositoryMock.Setup(repo => repo.GetByIdAsync(routeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(route);

        // Act
        var result = await _sut.GetRouteDetailsAsync(routeId);

        // Assert
        result.Should().NotBeNull();
        result!.RouteItems.Should().HaveCount(2);
        result.RouteItems.Should().ContainSingle(x => x.Order == 1);
        result.RouteItems.Should().ContainSingle(x => x.Order == 2);
        result.RouteItems[0].Address.Should().Be("Sarajevo, Test 1");
        result.RouteItems[1].Address.Should().Be("Sarajevo, Test 2");
    }

    [Fact]
    public async Task GetRouteDetailsAsync_ShouldPassCancellationToken()
    {
        // Arrange
        var routeId = Guid.NewGuid();
        var cancellationToken = new CancellationToken();

        _routeRepositoryMock.Setup(repo => repo.GetByIdAsync(routeId, cancellationToken))
            .ReturnsAsync((Route?)null);

        // Act
        await _sut.GetRouteDetailsAsync(routeId, cancellationToken);

        // Assert
        _routeRepositoryMock.Verify(repo => repo.GetByIdAsync(routeId, cancellationToken), Times.Once);
    }
}
