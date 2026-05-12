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

        // Standardna postavka repozitorija
        _routeRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Route>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Route r, CancellationToken ct) => { r.Id = Guid.NewGuid(); return r; });
    }

    [Fact]
    public async Task GenerateRouteAsync_ShouldReturnEmpty_WhenNoAvailableMailboxes()
    {
        // Arrange
        _mailboxRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Mailbox>());

        var request = new GenerateRouteRequest { Date = DateOnly.FromDateTime(DateTime.UtcNow), PlannedStartTime = new TimeOnly(8, 0, 0) };

        // Act
        var result = await _sut.GenerateRouteAsync(request);

        // Assert
        result.RouteItems.Should().BeEmpty();
    }

    [Fact]
    public async Task GenerateRouteAsync_ShouldFilterOutInactiveMailboxes()
    {
        // Arrange
        var mailboxes = new List<Mailbox>
        {
            new() { Id = Guid.NewGuid(), IsActive = false, WorkingDays = MailboxWorkingDays.SvakiDan, IsAlwaysAvailable = true, Latitude = 43.85m, Longitude = 18.41m, Priority = MailboxPriority.Visok }
        };
        _mailboxRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(mailboxes);
        var request = new GenerateRouteRequest { Date = DateOnly.FromDateTime(DateTime.UtcNow), PlannedStartTime = new TimeOnly(8, 0, 0) };

        // Act
        var result = await _sut.GenerateRouteAsync(request);

        // Assert
        result.RouteItems.Should().BeEmpty();
    }

    [Fact]
    public async Task GenerateRouteAsync_ShouldOnlySelectMailboxesMatchingWorkingDay()
    {
        // Arrange
        // Kreiraj datum na ponedjeljak
        var date = new DateOnly(2026, 5, 4); // Ponedjeljak
        
        var mailboxes = new List<Mailbox>
        {
            new() { Id = Guid.NewGuid(), IsActive = true, WorkingDays = MailboxWorkingDays.Ponedjeljak, IsAlwaysAvailable = true, Latitude = 43.85m, Longitude = 18.41m, Priority = MailboxPriority.Visok },
            new() { Id = Guid.NewGuid(), IsActive = true, WorkingDays = MailboxWorkingDays.Utorak, IsAlwaysAvailable = true, Latitude = 43.86m, Longitude = 18.42m, Priority = MailboxPriority.Visok }
        };
        _mailboxRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(mailboxes);
        var request = new GenerateRouteRequest { Date = date, PlannedStartTime = new TimeOnly(8, 0, 0) };

        // Act
        var result = await _sut.GenerateRouteAsync(request);

        // Assert
        result.RouteItems.Should().HaveCount(1);
        result.RouteItems.First().MailboxId.Should().Be(mailboxes[0].Id); // Samo onaj sa Ponedjeljkom prosao
    }

    [Fact]
    public async Task GenerateRouteAsync_ShouldExcludeMailboxesOutsideTimeWindow()
    {
        // Arrange
        var mailboxes = new List<Mailbox>
        {
            new() { 
                Id = Guid.NewGuid(), IsActive = true, WorkingDays = MailboxWorkingDays.SvakiDan, 
                IsAlwaysAvailable = false, 
                Slot1Start = new TimeOnly(12, 0), Slot1End = new TimeOnly(14, 0), // Prekasno za nas početak u 8
                Latitude = 43.85m, Longitude = 18.41m, Priority = MailboxPriority.Visok 
            }
        };
        _mailboxRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(mailboxes);
        var request = new GenerateRouteRequest { Date = DateOnly.FromDateTime(DateTime.UtcNow), PlannedStartTime = new TimeOnly(8, 0, 0) };

        // Act
        var result = await _sut.GenerateRouteAsync(request);

        // Assert
        result.RouteItems.Should().BeEmpty();
    }

    [Fact]
    public async Task GenerateRouteAsync_ShouldPrioritizeVisokOverNizak()
    {
        // Arrange
        // Depo je 43.8563, 18.4131. Nizak je odlicno blizu, ali visok mora ici prvi uprkos daljini
        var mailboxes = new List<Mailbox>
        {
            new() { Id = Guid.NewGuid(), IsActive = true, WorkingDays = MailboxWorkingDays.SvakiDan, IsAlwaysAvailable = true, Latitude = 43.8560m, Longitude = 18.4130m, Priority = MailboxPriority.Nizak },
            new() { Id = Guid.NewGuid(), IsActive = true, WorkingDays = MailboxWorkingDays.SvakiDan, IsAlwaysAvailable = true, Latitude = 44.0m, Longitude = 18.5m, Priority = MailboxPriority.Visok }
        };
        _mailboxRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(mailboxes);
        var request = new GenerateRouteRequest { Date = DateOnly.FromDateTime(DateTime.UtcNow), PlannedStartTime = new TimeOnly(8, 0, 0) };

        // Act
        var result = await _sut.GenerateRouteAsync(request);

        // Assert
        result.RouteItems.Should().HaveCount(2);
        result.RouteItems[0].MailboxId.Should().Be(mailboxes[1].Id); // Visok prioritet prvi
        result.RouteItems[1].MailboxId.Should().Be(mailboxes[0].Id);
    }
    
    [Fact]
    public async Task GenerateRouteAsync_ShouldSetExceedsStandardTimeWarning_IfLongerThan8Hours()
    {
        var mailboxes = new List<Mailbox>();
        
        // Dodaj 50 mailboxes vrlo daleko tako da travel time traje dugo
        for (int i = 0; i < 50; i++)
        {
            // Latitude razlika pomjera ~111km => ogromno vrijeme
            mailboxes.Add(new Mailbox { 
                Id = Guid.NewGuid(), IsActive = true, WorkingDays = MailboxWorkingDays.SvakiDan, IsAlwaysAvailable = true, 
                Latitude = 43.8560m + (i * 0.1m), Longitude = 18.4130m, 
                Priority = MailboxPriority.Srednji 
            });
        }
        
        _mailboxRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(mailboxes);
        var request = new GenerateRouteRequest { Date = DateOnly.FromDateTime(DateTime.UtcNow), PlannedStartTime = new TimeOnly(8, 0, 0) };

        // Act
        var result = await _sut.GenerateRouteAsync(request);

        // Assert
        result.ExceedsStandardTime.Should().BeTrue();
    }
}
