using Microsoft.EntityFrameworkCore;
using PostRoute.DAL.Entities;
using PostRoute.DAL.Repositories;
using Xunit;

namespace PostRoute.DAL.Tests.Repositories;

public sealed class RouteRepositoryTestsPBI024 : IDisposable
{
    private readonly AppDbContext _context;
    private readonly RouteRepository _sut;

    public RouteRepositoryTestsPBI024()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        _sut = new RouteRepository(_context);
    }

    private async Task SeedAsync()
    {
        var postmanId = Guid.NewGuid();
        var mailboxId = Guid.NewGuid();

        // Create postman first (foreign key constraint)
        var postman = new User
        {
            Id = postmanId,
            Email = "postman@test.com",
            FirstName = "Pera",
            LastName = "Perić",
            Username = "pera.peric",
            Role = "Postman",
            CreatedAt = DateTime.UtcNow
        };

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
            Id = Guid.NewGuid(),
            PostmanId = postmanId,
            Postman = postman,
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

        _context.Users.Add(postman);
        _context.Mailboxes.Add(mailbox);
        _context.Routes.Add(route);
        await _context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsRoute_WithIncludedItems()
    {
        // Arrange
        await SeedAsync();
        var route = _context.Routes.First();

        // Act
        var result = await _sut.GetByIdAsync(route.Id, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(route.Id, result.Id);
        Assert.Equal(route.PostmanId, result.PostmanId);
        Assert.Equal(route.Date, result.Date);
        Assert.Equal(route.PlannedStartTime, result.PlannedStartTime);
        Assert.Equal(route.PlannedEndTime, result.PlannedEndTime);
        Assert.Equal(route.TotalDistanceKm, result.TotalDistanceKm);
        Assert.Equal(route.TotalDurationMinutes, result.TotalDurationMinutes);
    }

    [Fact]
    public async Task GetByIdAsync_IncludesRouteItems()
    {
        // Arrange
        await SeedAsync();
        var route = _context.Routes.First();

        // Act
        var result = await _sut.GetByIdAsync(route.Id, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.RouteItems);
        Assert.Single(result.RouteItems);
        Assert.Equal(1, result.RouteItems.First().Order);
        Assert.Equal("Planirano", result.RouteItems.First().Status);
    }

    [Fact]
    public async Task GetByIdAsync_IncludesMailboxData()
    {
        // Arrange
        await SeedAsync();
        var route = _context.Routes.First();

        // Act
        var result = await _sut.GetByIdAsync(route.Id, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.RouteItems);
        var routeItem = result.RouteItems.First();
        Assert.NotNull(routeItem.Mailbox);
        Assert.Equal("SN001", routeItem.Mailbox.SerialNumber);
        Assert.Equal("Sarajevo, Test 1", routeItem.Mailbox.Address);
        Assert.Equal(43.85m, routeItem.Mailbox.Latitude);
        Assert.Equal(18.41m, routeItem.Mailbox.Longitude);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenRouteNotFound()
    {
        // Arrange
        await SeedAsync();
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _sut.GetByIdAsync(nonExistentId, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_IncludesPostmanData()
    {
        // Arrange
        var postmanId = Guid.NewGuid();
        var mailboxId = Guid.NewGuid();

        var postman = new User
        {
            Id = postmanId,
            Email = "postman@test.com",
            FirstName = "Pera",
            LastName = "Perić",
            Username = "pera.peric",
            Role = "Postman",
            CreatedAt = DateTime.UtcNow
        };

        var mailbox = new Mailbox
        {
            Id = mailboxId,
            SerialNumber = "SN002",
            Address = "Mostar, Test 2",
            Latitude = 43.34m,
            Longitude = 17.81m,
            Type = MailboxType.StandaloneLarge,
            Priority = MailboxPriority.Srednji,
            Status = MailboxStatus.Pun,
            Capacity = 200,
            InstallationYear = 2021
        };

        var route = new Route
        {
            Id = Guid.NewGuid(),
            PostmanId = postmanId,
            Postman = postman,
            Date = new DateOnly(2026, 5, 20),
            PlannedStartTime = new TimeOnly(9, 0),
            PlannedEndTime = new TimeOnly(13, 0),
            TotalDistanceKm = 25.0m,
            TotalDurationMinutes = 240,
            RouteItems = new List<RouteItem>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Order = 1,
                    MailboxId = mailboxId,
                    Mailbox = mailbox,
                    EstimatedArrivalTime = new TimeOnly(9, 30),
                    Status = "Planirano"
                }
            }
        };

        _context.Users.Add(postman);
        _context.Mailboxes.Add(mailbox);
        _context.Routes.Add(route);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetByIdAsync(route.Id, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Postman);
        Assert.Equal(postmanId, result.Postman.Id);
        Assert.Equal("Pera", result.Postman.FirstName);
        Assert.Equal("Perić", result.Postman.LastName);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
