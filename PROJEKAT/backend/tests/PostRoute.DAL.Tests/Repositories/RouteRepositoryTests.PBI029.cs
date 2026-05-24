using Microsoft.EntityFrameworkCore;
using PostRoute.DAL;
using PostRoute.DAL.Entities;
using PostRoute.DAL.Repositories;
using Xunit;

namespace PostRoute.DAL.Tests.Repositories;

public sealed class RouteRepositoryTestsPBI029 : IDisposable
{
    private readonly AppDbContext _context;
    private readonly RouteRepository _sut;

    public RouteRepositoryTestsPBI029()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _sut = new RouteRepository(_context);
    }

    private static User MakePostman(string username = "postar") => new()
    {
        Id = Guid.NewGuid(),
        FirstName = username,
        LastName = "Postar",
        Username = username,
        Email = $"{username}@postroute.ba",
        PasswordHash = "hash",
        Role = "PostalWorker"
    };

    private static Mailbox MakeMailbox() => new()
    {
        Id = Guid.NewGuid(),
        SerialNumber = Guid.NewGuid().ToString()[..8],
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

    private static Route MakeRoute(User postman, DateOnly date, RouteStatus status = RouteStatus.Dodijeljena) => new()
    {
        Id = Guid.NewGuid(),
        PostmanId = postman.Id,
        Postman = postman,
        Date = date,
        PlannedStartTime = new TimeOnly(8, 0),
        PlannedEndTime = new TimeOnly(10, 0),
        TotalDistanceKm = 10m,
        TotalDurationMinutes = 90,
        Status = status,
        RouteItems = new List<RouteItem>()
    };

    // ================================================================
    // FILTRIRANJE PO DATUMU
    // ================================================================

    [Fact]
    public async Task GetByDateAsync_ShouldReturnRoutes_ForGivenDate()
    {
        var targetDate = new DateOnly(2026, 5, 23);
        var postman1 = MakePostman("postar1");
        var postman2 = MakePostman("postar2");

        _context.Users.AddRange(postman1, postman2);
        _context.Routes.AddRange(
            MakeRoute(postman1, targetDate, RouteStatus.Dodijeljena),
            MakeRoute(postman2, targetDate, RouteStatus.UProgresu));
        await _context.SaveChangesAsync();

        var result = await _sut.GetByDateAsync(targetDate);

        Assert.Equal(2, result.Count);
        Assert.All(result, r => Assert.Equal(targetDate, r.Date));
    }

    [Fact]
    public async Task GetByDateAsync_ShouldExcludeRoutesForOtherDates()
    {
        var targetDate = new DateOnly(2026, 5, 23);
        var otherDate  = new DateOnly(2026, 5, 24);
        var postman1 = MakePostman("postar1");
        var postman2 = MakePostman("postar2");

        _context.Users.AddRange(postman1, postman2);
        _context.Routes.AddRange(
            MakeRoute(postman1, targetDate),
            MakeRoute(postman2, otherDate));
        await _context.SaveChangesAsync();

        var result = await _sut.GetByDateAsync(targetDate);

        Assert.Single(result);
        Assert.Equal(targetDate, result[0].Date);
    }

    [Fact]
    public async Task GetByDateAsync_ShouldReturnEmptyList_WhenNoRoutesForDate()
    {
        var result = await _sut.GetByDateAsync(new DateOnly(2026, 5, 23));

        Assert.Empty(result);
    }

    // ================================================================
    // EAGER LOADING
    // ================================================================

    [Fact]
    public async Task GetByDateAsync_ShouldIncludePostman_InResult()
    {
        var targetDate = new DateOnly(2026, 5, 23);
        var postman = MakePostman("amar");

        _context.Users.Add(postman);
        _context.Routes.Add(MakeRoute(postman, targetDate));
        await _context.SaveChangesAsync();

        var result = await _sut.GetByDateAsync(targetDate);

        Assert.NotNull(result[0].Postman);
        Assert.Equal("amar", result[0].Postman!.FirstName);
    }

    [Fact]
    public async Task GetByDateAsync_ShouldIncludeRouteItems_WithMailbox()
    {
        var targetDate = new DateOnly(2026, 5, 23);
        var postman = MakePostman("amar");
        var mailbox = MakeMailbox();
        var route = MakeRoute(postman, targetDate);

        route.RouteItems = new List<RouteItem>
        {
            new()
            {
                Id = Guid.NewGuid(),
                RouteId = route.Id,
                MailboxId = mailbox.Id,
                Mailbox = mailbox,
                Order = 1,
                EstimatedArrivalTime = new TimeOnly(8, 15),
                Status = "Planirano"
            }
        };

        _context.Users.Add(postman);
        _context.Mailboxes.Add(mailbox);
        _context.Routes.Add(route);
        await _context.SaveChangesAsync();

        var result = await _sut.GetByDateAsync(targetDate);

        Assert.Single(result[0].RouteItems);
        Assert.NotNull(result[0].RouteItems.First().Mailbox);
        Assert.Equal(mailbox.Address, result[0].RouteItems.First().Mailbox.Address);
    }

    // ================================================================
    // SORTIRANJE
    // ================================================================

    [Fact]
    public async Task GetByDateAsync_ShouldOrderBy_StatusThenPlannedStartTime()
    {
        var targetDate = new DateOnly(2026, 5, 23);
        var postman1 = MakePostman("postar1");
        var postman2 = MakePostman("postar2");
        var postman3 = MakePostman("postar3");

        var routeZavrsena   = MakeRoute(postman1, targetDate, RouteStatus.Zavrsena);
        var routeUProgresu  = MakeRoute(postman2, targetDate, RouteStatus.UProgresu);
        var routeDodijeljena = MakeRoute(postman3, targetDate, RouteStatus.Dodijeljena);

        routeZavrsena.PlannedStartTime    = new TimeOnly(8, 0);
        routeUProgresu.PlannedStartTime   = new TimeOnly(9, 0);
        routeDodijeljena.PlannedStartTime = new TimeOnly(7, 0);

        _context.Users.AddRange(postman1, postman2, postman3);
        _context.Routes.AddRange(routeZavrsena, routeUProgresu, routeDodijeljena);
        await _context.SaveChangesAsync();

        var result = await _sut.GetByDateAsync(targetDate);

        // RouteStatus enum: Planirana=0, UProgresu=1, Zavrsena=2, Otkazana=3, Dodijeljena=4
        // OrderBy Status — UProgresu(1) < Zavrsena(2) < Dodijeljena(4)
        Assert.Equal(RouteStatus.UProgresu,   result[0].Status);
        Assert.Equal(RouteStatus.Zavrsena,    result[1].Status);
        Assert.Equal(RouteStatus.Dodijeljena, result[2].Status);
    }

    // ================================================================
    // AKTIVNA RUTA PO SANDUCICU
    // ================================================================

    [Fact]
    public async Task GetActiveByPostmanAndMailboxAsync_ShouldReturnActiveRouteContainingMailbox()
    {
        var postman = MakePostman("postar1");
        var mailbox = MakeMailbox();
        var route = MakeRoute(postman, new DateOnly(2026, 5, 23), RouteStatus.Dodijeljena);
        route.RouteItems = new List<RouteItem>
        {
            new()
            {
                Id = Guid.NewGuid(),
                RouteId = route.Id,
                MailboxId = mailbox.Id,
                Mailbox = mailbox,
                Order = 1,
                EstimatedArrivalTime = new TimeOnly(8, 15),
                Status = "Planirano"
            }
        };

        _context.Users.Add(postman);
        _context.Mailboxes.Add(mailbox);
        _context.Routes.Add(route);
        await _context.SaveChangesAsync();

        var result = await _sut.GetActiveByPostmanAndMailboxAsync(postman.Id, mailbox.Id);

        Assert.NotNull(result);
        Assert.Equal(route.Id, result!.Id);
        Assert.Single(result.RouteItems);
        Assert.NotNull(result.RouteItems.First().Mailbox);
    }

    [Fact]
    public async Task GetActiveByPostmanAndMailboxAsync_ShouldIgnoreCompletedRoutes()
    {
        var postman = MakePostman("postar1");
        var mailbox = MakeMailbox();
        var route = MakeRoute(postman, new DateOnly(2026, 5, 23), RouteStatus.Zavrsena);
        route.RouteItems = new List<RouteItem>
        {
            new()
            {
                Id = Guid.NewGuid(),
                RouteId = route.Id,
                MailboxId = mailbox.Id,
                Mailbox = mailbox,
                Order = 1,
                EstimatedArrivalTime = new TimeOnly(8, 15),
                Status = "Obrađen"
            }
        };

        _context.Users.Add(postman);
        _context.Mailboxes.Add(mailbox);
        _context.Routes.Add(route);
        await _context.SaveChangesAsync();

        var result = await _sut.GetActiveByPostmanAndMailboxAsync(postman.Id, mailbox.Id);

        Assert.Null(result);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
