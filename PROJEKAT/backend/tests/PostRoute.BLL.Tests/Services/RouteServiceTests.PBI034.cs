using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using PostRoute.BLL.Services;
using PostRoute.DAL.Entities;
using PostRoute.DAL.Repositories;
using PostRoute.Domain.Entities;
using Xunit;

namespace PostRoute.BLL.Tests.Services;

public class RouteServiceTests_PBI034
{
    private readonly Mock<IRouteRepository> _mockRouteRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IMailboxRepository> _mockMailboxRepository;
    private readonly RouteService _routeService;

    public RouteServiceTests_PBI034()
    {
        _mockRouteRepository = new Mock<IRouteRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockMailboxRepository = new Mock<IMailboxRepository>();
        
        _routeService = new RouteService(
            _mockMailboxRepository.Object,
            _mockRouteRepository.Object,
            _mockUserRepository.Object
        );
    }

    [Fact]
    public async Task GetArchiveAsync_ReturnsPagedArchiveRoutes()
    {
        var routes = new List<Route>
        {
            new Route
            {
                Id = Guid.NewGuid(),
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
                Status = RouteStatus.Zavrsena,
                PostmanId = Guid.NewGuid(),
                TotalDistanceKm = 10,
                TotalDurationMinutes = 60,
                RouteItems = new List<RouteItem>()
            }
        };

        _mockRouteRepository
            .Setup(r => r.GetPagedArchiveAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<DateOnly?>(),
                It.IsAny<DateOnly?>(),
                It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((routes, 1));

        _mockUserRepository
            .Setup(u => u.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = routes[0].PostmanId, FirstName = "Postman", LastName = "Ili Postman" });

        var result = await _routeService.GetArchiveAsync(1, 10, null, null, null);

        result.Should().NotBeNull();
        result.TotalCount.Should().Be(1);
        result.Items.Should().HaveCount(1);
        result.Items[0].Id.Should().Be(routes[0].Id);
        result.Items[0].Status.Should().Be("Zavrsena");
    }

    [Fact]
    public async Task GetRouteDetailsAsync_ReturnsUnavailableReasonAndFinalStatus_ForArchivedRoute()
    {
        var mailbox = new Mailbox
        {
            Id = Guid.NewGuid(),
            SerialNumber = "SN-ARH-1",
            Address = "Titova 1",
            Latitude = 43.85m,
            Longitude = 18.41m,
            Type = MailboxType.WallSmall,
            Priority = MailboxPriority.Visok,
            Status = MailboxStatus.Prazan,
            Capacity = 100,
            InstallationYear = 2024
        };
        var route = new Route
        {
            Id = Guid.NewGuid(),
            Date = new DateOnly(2026, 5, 30),
            Status = RouteStatus.Zavrsena,
            PostmanId = Guid.NewGuid(),
            TotalDistanceKm = 10,
            TotalDurationMinutes = 60,
            RouteItems = new List<RouteItem>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Mailbox = mailbox,
                    MailboxId = mailbox.Id,
                    Order = 1,
                    EstimatedArrivalTime = new TimeOnly(8, 15),
                    Status = "Nedostupan",
                    ProcessedStatus = MailboxStatus.Nedostupan,
                    ProcessedAt = new DateTime(2026, 5, 30, 8, 20, 0, DateTimeKind.Utc),
                    UnavailableReason = "Zakljucan pristup"
                }
            }
        };

        _mockRouteRepository
            .Setup(r => r.GetByIdAsync(route.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(route);

        var result = await _routeService.GetRouteDetailsAsync(route.Id);

        result.Should().NotBeNull();
        result!.RouteItems.Should().ContainSingle();
        result.RouteItems[0].ProcessedStatus.Should().Be("Nedostupan");
        result.RouteItems[0].MailboxStatus.Should().Be("Nedostupan");
        result.RouteItems[0].ProcessedAt.Should().Be(route.RouteItems.First().ProcessedAt);
        result.RouteItems[0].UnavailableReason.Should().Be("Zakljucan pristup");
    }
}
