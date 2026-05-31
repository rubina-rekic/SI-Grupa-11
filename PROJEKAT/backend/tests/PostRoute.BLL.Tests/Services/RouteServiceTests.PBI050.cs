using FluentAssertions;
using Moq;
using PostRoute.BLL.Services;
using PostRoute.DAL.Entities;
using PostRoute.DAL.Repositories;
using PostRoute.Domain.Entities;

namespace PostRoute.BLL.Tests.Services;

public sealed class RouteServiceTestsPBI050
{
    private readonly Mock<IMailboxRepository> _mailboxRepositoryMock = new();
    private readonly Mock<IRouteRepository> _routeRepositoryMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly RouteService _sut;

    public RouteServiceTestsPBI050()
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

    private static RouteItem MakeItem(MailboxStatus? processedStatus) => new()
    {
        Id = Guid.NewGuid(),
        MailboxId = Guid.NewGuid(),
        Order = 1,
        EstimatedArrivalTime = new TimeOnly(8, 0),
        Status = processedStatus?.ToString() ?? "Planirano",
        ProcessedStatus = processedStatus,
        ProcessedAt = processedStatus.HasValue
            ? new DateTime(2026, 5, 10, 8, 15, 0, DateTimeKind.Utc)
            : null
    };

    private static Route MakeRoute(User postman, DateOnly date, params MailboxStatus?[] statuses) => new()
    {
        Id = Guid.NewGuid(),
        PostmanId = postman.Id,
        Postman = postman,
        Date = date,
        PlannedStartTime = new TimeOnly(8, 0),
        PlannedEndTime = new TimeOnly(10, 0),
        CompletedAt = new DateTime(date.Year, date.Month, date.Day, 10, 0, 0, DateTimeKind.Utc),
        Status = RouteStatus.Zavrsena,
        RouteItems = statuses.Select(MakeItem).ToList()
    };

    [Fact]
    public async Task GetPostmanPerformanceReportAsync_ShouldAggregateKpis_AndSortBySuccessDescending()
    {
        var fromDate = new DateOnly(2026, 5, 1);
        var toDate = new DateOnly(2026, 5, 31);
        var betterPostman = MakePostman("Ibrahim", "Test");
        var lowerPostman = MakePostman("Aldin", "Test");
        var routes = new List<Route>
        {
            MakeRoute(betterPostman, new DateOnly(2026, 5, 10), MailboxStatus.Ispraznjen, MailboxStatus.Ispraznjen, MailboxStatus.Nedostupan),
            MakeRoute(lowerPostman, new DateOnly(2026, 5, 12), MailboxStatus.Ispraznjen, MailboxStatus.Napunjen),
        };

        _routeRepositoryMock
            .Setup(r => r.GetCompletedRoutesForPerformanceReportAsync(fromDate, toDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(routes);

        var result = await _sut.GetPostmanPerformanceReportAsync(fromDate, toDate);

        result.TotalPostmen.Should().Be(2);
        result.TotalAssignedMailboxes.Should().Be(5);
        result.TotalEmptiedLocations.Should().Be(3);
        result.TotalUnrealizedLocations.Should().Be(2);
        result.Rows.Should().HaveCount(2);
        result.Rows[0].PostmanName.Should().Be("Ibrahim Test");
        result.Rows[0].AssignedMailboxes.Should().Be(3);
        result.Rows[0].EmptiedLocations.Should().Be(2);
        result.Rows[0].UnrealizedLocations.Should().Be(1);
        result.Rows[0].SuccessPercentage.Should().Be(66.67m);
        result.Rows[0].CompletedRoutesCount.Should().Be(1);
        result.Rows[0].Routes.Should().ContainSingle();
        result.Rows[1].SuccessPercentage.Should().Be(50m);
        result.TeamAverageSuccessPercentage.Should().Be(58.34m);
    }

    [Fact]
    public async Task GetPostmanPerformanceReportAsync_ShouldReturnEmptyRows_WhenNoCompletedRoutes()
    {
        var fromDate = new DateOnly(2026, 5, 1);
        var toDate = new DateOnly(2026, 5, 31);

        _routeRepositoryMock
            .Setup(r => r.GetCompletedRoutesForPerformanceReportAsync(fromDate, toDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Route>());

        var result = await _sut.GetPostmanPerformanceReportAsync(fromDate, toDate);

        result.Rows.Should().BeEmpty();
        result.TotalPostmen.Should().Be(0);
        result.TeamAverageSuccessPercentage.Should().Be(0);
    }

    [Fact]
    public async Task GetPostmanPerformanceReportAsync_ShouldRejectInvalidPeriod()
    {
        var act = () => _sut.GetPostmanPerformanceReportAsync(
            new DateOnly(2026, 5, 31),
            new DateOnly(2026, 5, 1));

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*datum*");
    }
}
