using FluentAssertions;
using Moq;
using PostRoute.BLL.Services;
using PostRoute.DAL.Entities;
using PostRoute.DAL.Repositories;
using PostRoute.Domain.Entities;

namespace PostRoute.BLL.Tests.Services;

public sealed class RouteServiceTestsPBI037
{
    private readonly Mock<IMailboxRepository> _mailboxRepositoryMock = new();
    private readonly Mock<IRouteRepository> _routeRepositoryMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly RouteService _sut;

    public RouteServiceTestsPBI037()
    {
        _sut = new RouteService(
            _mailboxRepositoryMock.Object,
            _routeRepositoryMock.Object,
            _userRepositoryMock.Object);
    }

    private static User MakePostman(string firstName, string lastName) => new()
    {
        Id = Guid.NewGuid(),
        FirstName = firstName,
        LastName = lastName,
        Username = $"{firstName.ToLowerInvariant()}.{lastName.ToLowerInvariant()}",
        Email = $"{firstName.ToLowerInvariant()}@postroute.ba",
        PasswordHash = "hash",
        Role = UserRole.PostalWorker
    };

    private static Mailbox MakeMailbox(int type, string address)
        => new()
        {
            Id = Guid.NewGuid(),
            Address = address,
            Latitude = 43.8563m,
            Longitude = 18.4131m,
            Type = (MailboxType)type,
            Priority = MailboxPriority.Srednji,
            Status = MailboxStatus.Prazan,
            Capacity = 10,
            InstallationYear = 2020,
            Notes = null,
            IsActive = true
        };

    private static RouteItem MakeItem(Mailbox mailbox, MailboxStatus? processedStatus, string status, string? notes = null)
        => new()
        {
            Id = Guid.NewGuid(),
            MailboxId = mailbox.Id,
            Mailbox = mailbox,
            Order = 1,
            EstimatedArrivalTime = new TimeOnly(8, 0),
            Status = status,
            ProcessedStatus = processedStatus,
            ProcessedAt = processedStatus.HasValue
                ? new DateTime(2026, 5, 10, 8, 15, 0, DateTimeKind.Utc)
                : null,
            UnavailableReason = notes
        };

    private static Route MakeRoute(User postman, DateOnly date, params RouteItem[] items)
        => new()
        {
            Id = Guid.NewGuid(),
            PostmanId = postman.Id,
            Postman = postman,
            Date = date,
            PlannedStartTime = new TimeOnly(8, 0),
            PlannedEndTime = new TimeOnly(10, 0),
            CompletedAt = new DateTime(date.Year, date.Month, date.Day, 10, 0, 0, DateTimeKind.Utc),
            Status = RouteStatus.Zavrsena,
            RouteItems = items.ToList()
        };

    [Fact]
    public async Task GetMailboxTypeRealizationReportAsync_ShouldGroupByMailboxType_AndCalculateFailureRate()
    {
        var fromDate = new DateOnly(2026, 5, 1);
        var toDate = new DateOnly(2026, 5, 31);

        var postman = MakePostman("Ibrahim", "Test");
        var wallMailbox = MakeMailbox(1, "Wall 1");
        var standMailbox = MakeMailbox(2, "Stand 1");

        var routes = new List<Route>
        {
            MakeRoute(postman, new DateOnly(2026, 5, 10),
                MakeItem(wallMailbox, MailboxStatus.Ispraznjen, nameof(MailboxStatus.Ispraznjen)),
                MakeItem(wallMailbox, MailboxStatus.Nedostupan, nameof(MailboxStatus.Nedostupan), "Nedostupan zbog ključa")),
            MakeRoute(postman, new DateOnly(2026, 5, 12),
                MakeItem(standMailbox, MailboxStatus.Ispraznjen, nameof(MailboxStatus.Ispraznjen)),
                MakeItem(standMailbox, null, "Planirano", "Nema pristupa"))
        };

        _routeRepositoryMock
            .Setup(r => r.GetCompletedRoutesForPerformanceReportAsync(fromDate, toDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(routes);

        var result = await _sut.GetMailboxTypeRealizationReportAsync(fromDate, toDate);

        result.TotalTypes.Should().Be(2);
        result.TotalPlannedEmpties.Should().Be(4);
        result.TotalSuccessfulEmpties.Should().Be(2);
        result.TotalProblemReports.Should().Be(2);
        result.AverageFailureRate.Should().Be(50m);

        result.Rows.Should().HaveCount(2);
        var wallRow = result.Rows.Single(r => r.TypeId == (int)MailboxType.WallSmall);
        wallRow.PlannedEmpties.Should().Be(2);
        wallRow.SuccessfulEmpties.Should().Be(1);
        wallRow.ProblemReports.Should().Be(1);
        wallRow.FailureRate.Should().Be(50m);
        wallRow.Details.Should().ContainSingle(d => d.Notes == "Nedostupan zbog ključa");
    }

    [Fact]
    public async Task GetMailboxTypeRealizationReportAsync_ShouldReturnEmptyRows_WhenNoCompletedRoutes()
    {
        var fromDate = new DateOnly(2026, 5, 1);
        var toDate = new DateOnly(2026, 5, 31);

        _routeRepositoryMock
            .Setup(r => r.GetCompletedRoutesForPerformanceReportAsync(fromDate, toDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Route>());

        var result = await _sut.GetMailboxTypeRealizationReportAsync(fromDate, toDate);

        result.Rows.Should().BeEmpty();
        result.TotalTypes.Should().Be(0);
        result.TotalPlannedEmpties.Should().Be(0);
        result.AverageFailureRate.Should().Be(0);
    }

    [Fact]
    public async Task GetMailboxTypeRealizationReportAsync_ShouldRejectInvalidPeriod()
    {
        var act = () => _sut.GetMailboxTypeRealizationReportAsync(
            new DateOnly(2026, 5, 31),
            new DateOnly(2026, 5, 1));

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*datum*");
    }
}
