using Microsoft.EntityFrameworkCore;
using PostRoute.DAL;
using PostRoute.DAL.Entities;
using PostRoute.DAL.Repositories;
using Xunit;

namespace PostRoute.DAL.Tests.Repositories;

public sealed class RouteRepositoryTestsPBI023 : IDisposable
{
    private readonly AppDbContext _context;
    private readonly RouteRepository _sut;

    public RouteRepositoryTestsPBI023()
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

    private static Route MakeRoute(User postman, DateOnly date, RouteStatus status) => new()
    {
        Id = Guid.NewGuid(),
        PostmanId = postman.Id,
        Postman = postman,
        Date = date,
        PlannedStartTime = new TimeOnly(8, 0),
        PlannedEndTime = new TimeOnly(9, 0),
        Status = status
    };

    [Fact]
    public async Task GetPostmanIdsWithActiveRouteOnDateAsync_ReturnsAssignedAndInProgressRoutesOnly()
    {
        var date = new DateOnly(2026, 5, 20);
        var assignedPostman = MakePostman("assigned");
        var inProgressPostman = MakePostman("progress");
        var plannedPostman = MakePostman("planned");
        var canceledPostman = MakePostman("canceled");

        _context.Routes.AddRange(
            MakeRoute(assignedPostman, date, RouteStatus.Dodijeljena),
            MakeRoute(inProgressPostman, date, RouteStatus.UProgresu),
            MakeRoute(plannedPostman, date, RouteStatus.Planirana),
            MakeRoute(canceledPostman, date, RouteStatus.Otkazana));
        await _context.SaveChangesAsync();

        var result = await _sut.GetPostmanIdsWithActiveRouteOnDateAsync(date);

        Assert.Contains(assignedPostman.Id, result);
        Assert.Contains(inProgressPostman.Id, result);
        Assert.DoesNotContain(plannedPostman.Id, result);
        Assert.DoesNotContain(canceledPostman.Id, result);
    }

    [Fact]
    public async Task GetPostmanIdsWithActiveRouteOnDateAsync_ExcludesCurrentRoute()
    {
        var date = new DateOnly(2026, 5, 20);
        var currentPostman = MakePostman("current");
        var otherPostman = MakePostman("other");
        var currentRoute = MakeRoute(currentPostman, date, RouteStatus.Dodijeljena);
        var otherRoute = MakeRoute(otherPostman, date, RouteStatus.Dodijeljena);

        _context.Routes.AddRange(currentRoute, otherRoute);
        await _context.SaveChangesAsync();

        var result = await _sut.GetPostmanIdsWithActiveRouteOnDateAsync(date, currentRoute.Id);

        Assert.DoesNotContain(currentPostman.Id, result);
        Assert.Contains(otherPostman.Id, result);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
