using Microsoft.EntityFrameworkCore;
using PostRoute.DAL;
using PostRoute.DAL.Entities;
using PostRoute.DAL.Repositories;
using Xunit;

namespace PostRoute.DAL.Tests.Repositories;

public sealed class RouteRepositoryTestsPBI050 : IDisposable
{
    private readonly AppDbContext _context;
    private readonly RouteRepository _sut;

    public RouteRepositoryTestsPBI050()
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

    private static Mailbox MakeMailbox(string serial) => new()
    {
        Id = Guid.NewGuid(),
        SerialNumber = serial,
        Address = $"Adresa {serial}",
        Latitude = 43.85m,
        Longitude = 18.41m,
        Type = MailboxType.WallSmall,
        Priority = MailboxPriority.Srednji,
        Status = MailboxStatus.Prazan,
        Capacity = 100,
        InstallationYear = 2024
    };

    private static Route MakeRoute(User postman, DateOnly date, RouteStatus status, Mailbox mailbox) => new()
    {
        Id = Guid.NewGuid(),
        PostmanId = postman.Id,
        Postman = postman,
        Date = date,
        PlannedStartTime = new TimeOnly(8, 0),
        PlannedEndTime = new TimeOnly(10, 0),
        TotalDistanceKm = 10m,
        TotalDurationMinutes = 120,
        Status = status,
        RouteItems = new List<RouteItem>
        {
            new()
            {
                Id = Guid.NewGuid(),
                MailboxId = mailbox.Id,
                Mailbox = mailbox,
                Order = 1,
                EstimatedArrivalTime = new TimeOnly(8, 15),
                Status = "Ispraznjen",
                ProcessedStatus = MailboxStatus.Ispraznjen
            }
        }
    };

    [Fact]
    public async Task GetCompletedRoutesForPerformanceReportAsync_ShouldReturnOnlyCompletedRoutesInPeriod()
    {
        var postman = MakePostman("ibrahim");
        var mailbox1 = MakeMailbox("SN-1");
        var mailbox2 = MakeMailbox("SN-2");
        var mailbox3 = MakeMailbox("SN-3");
        var completedInPeriod = MakeRoute(postman, new DateOnly(2026, 5, 10), RouteStatus.Zavrsena, mailbox1);
        var completedOutsidePeriod = MakeRoute(postman, new DateOnly(2026, 4, 30), RouteStatus.Zavrsena, mailbox2);
        var canceledInPeriod = MakeRoute(postman, new DateOnly(2026, 5, 11), RouteStatus.Otkazana, mailbox3);

        _context.Users.Add(postman);
        _context.Mailboxes.AddRange(mailbox1, mailbox2, mailbox3);
        _context.Routes.AddRange(completedInPeriod, completedOutsidePeriod, canceledInPeriod);
        await _context.SaveChangesAsync();

        var result = await _sut.GetCompletedRoutesForPerformanceReportAsync(
            new DateOnly(2026, 5, 1),
            new DateOnly(2026, 5, 31));

        Assert.Single(result);
        Assert.Equal(completedInPeriod.Id, result[0].Id);
        Assert.NotNull(result[0].Postman);
        Assert.Single(result[0].RouteItems);
        Assert.NotNull(result[0].RouteItems.First().Mailbox);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
