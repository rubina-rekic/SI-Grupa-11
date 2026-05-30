using Microsoft.EntityFrameworkCore;
using PostRoute.DAL;
using PostRoute.DAL.Entities;
using PostRoute.DAL.Repositories;
using Xunit;

namespace PostRoute.DAL.Tests.Repositories;

public sealed class RouteRepositoryTestsPBI034 : IDisposable
{
    private readonly AppDbContext _context;
    private readonly RouteRepository _sut;

    public RouteRepositoryTestsPBI034()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _sut = new RouteRepository(_context);
    }

    private static User MakePostman(string username) => new()
    {
        Id = Guid.NewGuid(),
        FirstName = username,
        LastName = "Postar",
        Username = username,
        Email = $"{username}@postroute.ba",
        PasswordHash = "hash",
        Role = "PostalWorker"
    };

    private static Route MakeRoute(User postman, DateOnly date, RouteStatus status, int hour = 8) => new()
    {
        Id = Guid.NewGuid(),
        PostmanId = postman.Id,
        Postman = postman,
        Date = date,
        PlannedStartTime = new TimeOnly(hour, 0),
        PlannedEndTime = new TimeOnly(hour + 1, 0),
        TotalDistanceKm = 10m,
        TotalDurationMinutes = 60,
        Status = status,
        RouteItems = new List<RouteItem>()
    };

    [Fact]
    public async Task GetPagedArchiveAsync_ReturnsOnlyCompletedAndCancelledRoutes()
    {
        var postman = MakePostman("archive");
        _context.Users.Add(postman);
        _context.Routes.AddRange(
            MakeRoute(postman, new DateOnly(2026, 5, 30), RouteStatus.Zavrsena),
            MakeRoute(postman, new DateOnly(2026, 5, 29), RouteStatus.Otkazana),
            MakeRoute(postman, new DateOnly(2026, 5, 28), RouteStatus.UProgresu),
            MakeRoute(postman, new DateOnly(2026, 5, 27), RouteStatus.Dodijeljena));
        await _context.SaveChangesAsync();

        var (items, total) = await _sut.GetPagedArchiveAsync(1, 25, null, null, null);

        Assert.Equal(2, total);
        Assert.All(items, route => Assert.Contains(route.Status, new[] { RouteStatus.Zavrsena, RouteStatus.Otkazana }));
    }

    [Fact]
    public async Task GetPagedArchiveAsync_FiltersByDateRangeAndPostman()
    {
        var selectedPostman = MakePostman("selected");
        var otherPostman = MakePostman("other");
        _context.Users.AddRange(selectedPostman, otherPostman);
        _context.Routes.AddRange(
            MakeRoute(selectedPostman, new DateOnly(2026, 5, 30), RouteStatus.Zavrsena),
            MakeRoute(selectedPostman, new DateOnly(2026, 5, 20), RouteStatus.Zavrsena),
            MakeRoute(otherPostman, new DateOnly(2026, 5, 30), RouteStatus.Zavrsena));
        await _context.SaveChangesAsync();

        var (items, total) = await _sut.GetPagedArchiveAsync(
            1,
            25,
            new DateOnly(2026, 5, 25),
            new DateOnly(2026, 5, 31),
            selectedPostman.Id);

        Assert.Equal(1, total);
        Assert.Single(items);
        Assert.Equal(selectedPostman.Id, items[0].PostmanId);
        Assert.Equal(new DateOnly(2026, 5, 30), items[0].Date);
    }

    [Fact]
    public async Task GetPagedArchiveAsync_OrdersNewestToOldest()
    {
        var postman = MakePostman("order");
        var older = MakeRoute(postman, new DateOnly(2026, 5, 29), RouteStatus.Zavrsena, 12);
        var newestMorning = MakeRoute(postman, new DateOnly(2026, 5, 30), RouteStatus.Zavrsena, 8);
        var newestAfternoon = MakeRoute(postman, new DateOnly(2026, 5, 30), RouteStatus.Zavrsena, 14);
        _context.Users.Add(postman);
        _context.Routes.AddRange(older, newestMorning, newestAfternoon);
        await _context.SaveChangesAsync();

        var (items, _) = await _sut.GetPagedArchiveAsync(1, 25, null, null, null);

        Assert.Equal(newestAfternoon.Id, items[0].Id);
        Assert.Equal(newestMorning.Id, items[1].Id);
        Assert.Equal(older.Id, items[2].Id);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
