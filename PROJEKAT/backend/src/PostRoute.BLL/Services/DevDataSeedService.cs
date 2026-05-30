using PostRoute.DAL;
using PostRoute.DAL.Entities;

namespace PostRoute.BLL.Services;

public sealed class DevDataSeedService : IDevDataSeedService
{
    private const string CompletedMailboxSerial1 = "DEV-ARCH-001";
    private const string CompletedMailboxSerial2 = "DEV-ARCH-002";
    private readonly AppDbContext _dbContext;

    public DevDataSeedService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SeedArchiveRouteAsync(CancellationToken cancellationToken)
    {
        var postman = _dbContext.Users.FirstOrDefault(user => user.Username == "postar");
        if (postman is null)
        {
            return;
        }

        var today = DateOnly.FromDateTime(DateTime.Now);
        var archiveRouteAlreadySeeded = _dbContext.RouteItems.Any(item =>
            item.Route.Date == today &&
            item.Route.Status == RouteStatus.Zavrsena &&
            (item.Mailbox.SerialNumber == CompletedMailboxSerial1 ||
             item.Mailbox.SerialNumber == CompletedMailboxSerial2));

        if (archiveRouteAlreadySeeded)
        {
            return;
        }

        var now = DateTime.UtcNow;
        var mailbox1 = EnsureMailbox(
            CompletedMailboxSerial1,
            "Dev arhiva - Titova 1",
            43.856300m,
            18.413100m,
            MailboxPriority.Visok,
            MailboxStatus.Ispraznjen,
            now);
        var mailbox2 = EnsureMailbox(
            CompletedMailboxSerial2,
            "Dev arhiva - Zmaja od Bosne 2",
            43.858000m,
            18.410000m,
            MailboxPriority.Srednji,
            MailboxStatus.Napunjen,
            now);

        var startedAt = new DateTime(
            today.Year,
            today.Month,
            today.Day,
            8,
            0,
            0,
            DateTimeKind.Utc);
        var completedAt = startedAt.AddMinutes(35);

        _dbContext.Routes.Add(new Route
        {
            Id = Guid.NewGuid(),
            PostmanId = postman.Id,
            Postman = postman,
            Date = today,
            PlannedStartTime = new TimeOnly(8, 0),
            PlannedEndTime = new TimeOnly(8, 35),
            TotalDistanceKm = 3.25m,
            TotalDurationMinutes = 35,
            Status = RouteStatus.Zavrsena,
            ExceedsStandardTime = false,
            AssignedAt = startedAt.AddMinutes(-20),
            AssignedBy = "development-seed",
            StartedAt = startedAt,
            CompletedAt = completedAt,
            CreatedAt = now,
            RouteItems = new List<RouteItem>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Mailbox = mailbox1,
                    MailboxId = mailbox1.Id,
                    Order = 1,
                    EstimatedArrivalTime = new TimeOnly(8, 10),
                    Status = "Obrađen",
                    ProcessedAt = startedAt.AddMinutes(10),
                    ProcessedBy = postman.Id,
                    ProcessedStatus = MailboxStatus.Ispraznjen
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Mailbox = mailbox2,
                    MailboxId = mailbox2.Id,
                    Order = 2,
                    EstimatedArrivalTime = new TimeOnly(8, 25),
                    Status = "Obrađen",
                    ProcessedAt = completedAt,
                    ProcessedBy = postman.Id,
                    ProcessedStatus = MailboxStatus.Napunjen
                }
            }
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private Mailbox EnsureMailbox(
        string serialNumber,
        string address,
        decimal latitude,
        decimal longitude,
        MailboxPriority priority,
        MailboxStatus status,
        DateTime timestamp)
    {
        var mailbox = _dbContext.Mailboxes.FirstOrDefault(m => m.SerialNumber == serialNumber);
        if (mailbox is not null)
        {
            return mailbox;
        }

        mailbox = new Mailbox
        {
            Id = Guid.NewGuid(),
            SerialNumber = serialNumber,
            Address = address,
            Latitude = latitude,
            Longitude = longitude,
            Type = MailboxType.WallSmall,
            Priority = priority,
            Status = status,
            Capacity = 100,
            InstallationYear = 2024,
            IsAlwaysAvailable = true,
            WorkingDays = MailboxWorkingDays.SvakiDan,
            IsActive = true,
            CreatedAt = timestamp,
            UpdatedAt = timestamp,
            Notes = "Development seed data for archive verification."
        };

        _dbContext.Mailboxes.Add(mailbox);
        return mailbox;
    }
}
