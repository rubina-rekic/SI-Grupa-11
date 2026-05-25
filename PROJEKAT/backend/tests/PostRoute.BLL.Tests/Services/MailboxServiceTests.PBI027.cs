using Moq;
using PostRoute.BLL.Commands;
using PostRoute.BLL.Services;
using PostRoute.DAL.Entities;
using PostRoute.DAL.Repositories;
using Xunit;

namespace PostRoute.BLL.Tests.Services;

/// <summary>
/// Unit testovi za PBI-027 (US-27): Ažuriranje statusa sandučića
/// Pokriva: UpdateStatusAsync — promjena statusa, audit log, validacija
/// </summary>
public sealed class MailboxServiceTestsPBI027
{
    private readonly Mock<IMailboxRepository> _repo;
    private readonly Mock<IMailboxAuditLogRepository> _auditRepo;
    private readonly MailboxService _sut;

    public MailboxServiceTestsPBI027()
    {
        _repo = new Mock<IMailboxRepository>();
        _auditRepo = new Mock<IMailboxAuditLogRepository>();
        _sut = new MailboxService(_repo.Object, _auditRepo.Object);
    }

    private static Mailbox MakeMailbox(MailboxStatus status = MailboxStatus.Prazan) => new()
    {
        Id = Guid.NewGuid(),
        SerialNumber = "SN-027",
        Address = "Testna adresa 1",
        Latitude = 43.85m,
        Longitude = 18.41m,
        Type = MailboxType.WallSmall,
        Priority = MailboxPriority.Srednji,
        Status = status,
        Capacity = 100,
        InstallationYear = 2024,
        WorkingDays = MailboxWorkingDays.RadniDani,
        IsAlwaysAvailable = true
    };

    // ================================================================
    // HAPPY PATH
    // ================================================================

    [Fact]
    public async Task UpdateStatusAsync_ShouldUpdateStatus_WhenMailboxExists()
    {
        var mailbox = MakeMailbox(MailboxStatus.Prazan);
        var userId = Guid.NewGuid();

        _repo.Setup(r => r.GetByIdAsync(mailbox.Id, It.IsAny<CancellationToken>()))
             .ReturnsAsync(mailbox);
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Mailbox>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((Mailbox m, CancellationToken _) => m);

        var command = new UpdateMailboxStatusCommand(mailbox.Id, MailboxStatus.Napunjen, userId);
        var result = await _sut.UpdateStatusAsync(command, CancellationToken.None);

        Assert.Equal(MailboxStatus.Napunjen, result.Status);
        _repo.Verify(r => r.UpdateAsync(It.Is<Mailbox>(m => m.Status == MailboxStatus.Napunjen), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(MailboxStatus.Obraen)]
    [InlineData(MailboxStatus.Napunjen)]
    [InlineData(MailboxStatus.Ispraznjen)]
    [InlineData(MailboxStatus.Pun)]
    [InlineData(MailboxStatus.Prazan)]
    public async Task UpdateStatusAsync_ShouldAcceptAllValidStatuses(MailboxStatus newStatus)
    {
        var mailbox = MakeMailbox(MailboxStatus.Prazan);
        var userId = Guid.NewGuid();

        _repo.Setup(r => r.GetByIdAsync(mailbox.Id, It.IsAny<CancellationToken>()))
             .ReturnsAsync(mailbox);
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Mailbox>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((Mailbox m, CancellationToken _) => m);

        var command = new UpdateMailboxStatusCommand(mailbox.Id, newStatus, userId);
        var result = await _sut.UpdateStatusAsync(command, CancellationToken.None);

        Assert.Equal(newStatus, result.Status);
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldUpdateUpdatedAt()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var mailbox = MakeMailbox();
        var userId = Guid.NewGuid();

        _repo.Setup(r => r.GetByIdAsync(mailbox.Id, It.IsAny<CancellationToken>()))
             .ReturnsAsync(mailbox);
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Mailbox>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((Mailbox m, CancellationToken _) => m);

        var command = new UpdateMailboxStatusCommand(mailbox.Id, MailboxStatus.Ispraznjen, userId);
        var result = await _sut.UpdateStatusAsync(command, CancellationToken.None);

        Assert.True(result.UpdatedAt >= before);
    }

    // ================================================================
    // AUDIT LOG
    // ================================================================

    [Fact]
    public async Task UpdateStatusAsync_ShouldLogAuditEntry_WithCorrectFields()
    {
        var mailbox = MakeMailbox(MailboxStatus.Prazan);
        var userId = Guid.NewGuid();
        MailboxAuditLog? captured = null;

        _repo.Setup(r => r.GetByIdAsync(mailbox.Id, It.IsAny<CancellationToken>()))
             .ReturnsAsync(mailbox);
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Mailbox>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((Mailbox m, CancellationToken _) => m);
        _auditRepo.Setup(a => a.LogAsync(It.IsAny<MailboxAuditLog>(), It.IsAny<CancellationToken>()))
                  .Callback<MailboxAuditLog, CancellationToken>((log, _) => captured = log)
                  .Returns(Task.CompletedTask);

        var command = new UpdateMailboxStatusCommand(mailbox.Id, MailboxStatus.Obraen, userId);
        await _sut.UpdateStatusAsync(command, CancellationToken.None);

        Assert.NotNull(captured);
        Assert.Equal(mailbox.Id, captured!.MailboxId);
        Assert.Equal(userId, captured.UserId);
        Assert.Equal("Status", captured.FieldName);
        Assert.Equal("Prazan", captured.OldValue);
        Assert.Equal("Obraen", captured.NewValue);
        Assert.Equal("UPDATE", captured.Action);
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldLogAuditEntry_WithReason_WhenProvided()
    {
        var mailbox = MakeMailbox(MailboxStatus.Prazan);
        var userId = Guid.NewGuid();
        MailboxAuditLog? captured = null;

        _repo.Setup(r => r.GetByIdAsync(mailbox.Id, It.IsAny<CancellationToken>()))
             .ReturnsAsync(mailbox);
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Mailbox>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((Mailbox m, CancellationToken _) => m);
        _auditRepo.Setup(a => a.LogAsync(It.IsAny<MailboxAuditLog>(), It.IsAny<CancellationToken>()))
                  .Callback<MailboxAuditLog, CancellationToken>((log, _) => captured = log)
                  .Returns(Task.CompletedTask);

        var command = new UpdateMailboxStatusCommand(mailbox.Id, MailboxStatus.Napunjen, userId, "Sandučić je pun tokom obilaska");
        await _sut.UpdateStatusAsync(command, CancellationToken.None);

        Assert.NotNull(captured);
        Assert.Equal("Sandučić je pun tokom obilaska", captured!.Reason);
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldLogAuditEntry_WithNullReason_WhenNotProvided()
    {
        var mailbox = MakeMailbox(MailboxStatus.Prazan);
        MailboxAuditLog? captured = null;

        _repo.Setup(r => r.GetByIdAsync(mailbox.Id, It.IsAny<CancellationToken>()))
             .ReturnsAsync(mailbox);
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Mailbox>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((Mailbox m, CancellationToken _) => m);
        _auditRepo.Setup(a => a.LogAsync(It.IsAny<MailboxAuditLog>(), It.IsAny<CancellationToken>()))
                  .Callback<MailboxAuditLog, CancellationToken>((log, _) => captured = log)
                  .Returns(Task.CompletedTask);

        var command = new UpdateMailboxStatusCommand(mailbox.Id, MailboxStatus.Ispraznjen, Guid.NewGuid());
        await _sut.UpdateStatusAsync(command, CancellationToken.None);

        Assert.NotNull(captured);
        Assert.Null(captured!.Reason);
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldLogAuditEntry_WithTimestamp()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var mailbox = MakeMailbox();
        MailboxAuditLog? captured = null;

        _repo.Setup(r => r.GetByIdAsync(mailbox.Id, It.IsAny<CancellationToken>()))
             .ReturnsAsync(mailbox);
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Mailbox>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((Mailbox m, CancellationToken _) => m);
        _auditRepo.Setup(a => a.LogAsync(It.IsAny<MailboxAuditLog>(), It.IsAny<CancellationToken>()))
                  .Callback<MailboxAuditLog, CancellationToken>((log, _) => captured = log)
                  .Returns(Task.CompletedTask);

        await _sut.UpdateStatusAsync(new UpdateMailboxStatusCommand(mailbox.Id, MailboxStatus.Obraen, Guid.NewGuid()), CancellationToken.None);

        Assert.NotNull(captured);
        Assert.True(captured!.Timestamp >= before);
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldCallAuditLog_ExactlyOnce()
    {
        var mailbox = MakeMailbox();

        _repo.Setup(r => r.GetByIdAsync(mailbox.Id, It.IsAny<CancellationToken>()))
             .ReturnsAsync(mailbox);
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Mailbox>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((Mailbox m, CancellationToken _) => m);

        await _sut.UpdateStatusAsync(new UpdateMailboxStatusCommand(mailbox.Id, MailboxStatus.Napunjen, Guid.NewGuid()), CancellationToken.None);

        _auditRepo.Verify(a => a.LogAsync(It.IsAny<MailboxAuditLog>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldUpdateRouteItemAndStartRoute_WhenAssignedRouteExists()
    {
        var mailbox = MakeMailbox(MailboxStatus.Pun);
        var userId = Guid.NewGuid();
        var today = DateOnly.FromDateTime(DateTime.Now);
        var route = new Route
        {
            Id = Guid.NewGuid(),
            PostmanId = userId,
            Date = today,
            PlannedStartTime = new TimeOnly(8, 0),
            Status = RouteStatus.Dodijeljena,
            RouteItems = new List<RouteItem>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    MailboxId = mailbox.Id,
                    Mailbox = mailbox,
                    Order = 1,
                    EstimatedArrivalTime = new TimeOnly(8, 15),
                    Status = "Planirano"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    MailboxId = Guid.NewGuid(),
                    Mailbox = MakeMailbox(MailboxStatus.Prazan),
                    Order = 2,
                    EstimatedArrivalTime = new TimeOnly(8, 30),
                    Status = "Planirano"
                }
            }
        };
        var routeRepo = new Mock<IRouteRepository>();
        var sut = new MailboxService(_repo.Object, _auditRepo.Object, routeRepo.Object);

        _repo.Setup(r => r.GetByIdAsync(mailbox.Id, It.IsAny<CancellationToken>()))
             .ReturnsAsync(mailbox);
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Mailbox>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((Mailbox m, CancellationToken _) => m);
        routeRepo.Setup(r => r.GetByPostmanAndDateAsync(userId, today, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(route);
        routeRepo.Setup(r => r.UpdateAsync(It.IsAny<Route>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync((Route r, CancellationToken _) => r);

        await sut.UpdateStatusAsync(new UpdateMailboxStatusCommand(mailbox.Id, MailboxStatus.Ispraznjen, userId), CancellationToken.None);

        var routeItem = route.RouteItems.Single(i => i.MailboxId == mailbox.Id);
        Assert.Equal("Obrađen", routeItem.Status);
        Assert.Equal(MailboxStatus.Ispraznjen, routeItem.ProcessedStatus);
        Assert.Equal(userId, routeItem.ProcessedBy);
        Assert.NotNull(routeItem.ProcessedAt);
        Assert.Equal(RouteStatus.UProgresu, route.Status);
        Assert.NotNull(route.StartedAt);
        Assert.Null(route.CompletedAt);
        routeRepo.Verify(r => r.UpdateAsync(route, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldCompleteRoute_WhenLastRouteItemIsProcessed()
    {
        var mailbox = MakeMailbox(MailboxStatus.Prazan);
        var userId = Guid.NewGuid();
        var today = DateOnly.FromDateTime(DateTime.Now);
        var route = new Route
        {
            Id = Guid.NewGuid(),
            PostmanId = userId,
            Date = today,
            PlannedStartTime = new TimeOnly(8, 0),
            Status = RouteStatus.Dodijeljena,
            RouteItems = new List<RouteItem>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    MailboxId = mailbox.Id,
                    Mailbox = mailbox,
                    Order = 1,
                    EstimatedArrivalTime = new TimeOnly(8, 15),
                    Status = "Planirano"
                }
            }
        };
        var routeRepo = new Mock<IRouteRepository>();
        var sut = new MailboxService(_repo.Object, _auditRepo.Object, routeRepo.Object);

        _repo.Setup(r => r.GetByIdAsync(mailbox.Id, It.IsAny<CancellationToken>()))
             .ReturnsAsync(mailbox);
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Mailbox>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((Mailbox m, CancellationToken _) => m);
        routeRepo.Setup(r => r.GetByPostmanAndDateAsync(userId, today, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(route);
        routeRepo.Setup(r => r.UpdateAsync(It.IsAny<Route>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync((Route r, CancellationToken _) => r);

        await sut.UpdateStatusAsync(new UpdateMailboxStatusCommand(mailbox.Id, MailboxStatus.Napunjen, userId), CancellationToken.None);

        Assert.Equal(RouteStatus.Zavrsena, route.Status);
        Assert.NotNull(route.StartedAt);
        Assert.NotNull(route.CompletedAt);
        Assert.Equal(MailboxStatus.Napunjen, route.RouteItems.Single().ProcessedStatus);
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldFindActiveRouteByMailbox_WhenDateLookupMisses()
    {
        var mailbox = MakeMailbox(MailboxStatus.Prazan);
        var userId = Guid.NewGuid();
        var route = new Route
        {
            Id = Guid.NewGuid(),
            PostmanId = userId,
            Date = new DateOnly(2026, 5, 23),
            PlannedStartTime = new TimeOnly(8, 0),
            Status = RouteStatus.Dodijeljena,
            RouteItems = new List<RouteItem>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    MailboxId = mailbox.Id,
                    Mailbox = mailbox,
                    Order = 1,
                    EstimatedArrivalTime = new TimeOnly(8, 15),
                    Status = "Planirano"
                }
            }
        };
        var routeRepo = new Mock<IRouteRepository>();
        var sut = new MailboxService(_repo.Object, _auditRepo.Object, routeRepo.Object);

        _repo.Setup(r => r.GetByIdAsync(mailbox.Id, It.IsAny<CancellationToken>()))
             .ReturnsAsync(mailbox);
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Mailbox>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((Mailbox m, CancellationToken _) => m);
        routeRepo.Setup(r => r.GetByPostmanAndDateAsync(userId, It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync((Route?)null);
        routeRepo.Setup(r => r.GetActiveByPostmanAndMailboxAsync(userId, mailbox.Id, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(route);
        routeRepo.Setup(r => r.UpdateAsync(It.IsAny<Route>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync((Route r, CancellationToken _) => r);

        await sut.UpdateStatusAsync(new UpdateMailboxStatusCommand(mailbox.Id, MailboxStatus.Napunjen, userId), CancellationToken.None);

        Assert.Equal(RouteStatus.Zavrsena, route.Status);
        Assert.NotNull(route.CompletedAt);
        Assert.Equal(MailboxStatus.Napunjen, route.RouteItems.Single().ProcessedStatus);
        routeRepo.Verify(r => r.GetActiveByPostmanAndMailboxAsync(userId, mailbox.Id, It.IsAny<CancellationToken>()), Times.Once);
        routeRepo.Verify(r => r.UpdateAsync(route, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldReject_WhenRouteItemAlreadyProcessed()
    {
        var mailbox = MakeMailbox(MailboxStatus.Prazan);
        var userId = Guid.NewGuid();
        var today = DateOnly.FromDateTime(DateTime.Now);
        var route = new Route
        {
            Id = Guid.NewGuid(),
            PostmanId = userId,
            Date = today,
            PlannedStartTime = new TimeOnly(8, 0),
            Status = RouteStatus.UProgresu,
            RouteItems = new List<RouteItem>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    MailboxId = mailbox.Id,
                    Mailbox = mailbox,
                    Order = 1,
                    EstimatedArrivalTime = new TimeOnly(8, 15),
                    Status = "Obrađen",
                    ProcessedAt = DateTime.UtcNow.AddMinutes(-5),
                    ProcessedBy = userId,
                    ProcessedStatus = MailboxStatus.Ispraznjen
                }
            }
        };
        var routeRepo = new Mock<IRouteRepository>();
        var sut = new MailboxService(_repo.Object, _auditRepo.Object, routeRepo.Object);

        _repo.Setup(r => r.GetByIdAsync(mailbox.Id, It.IsAny<CancellationToken>()))
             .ReturnsAsync(mailbox);
        routeRepo.Setup(r => r.GetByPostmanAndDateAsync(userId, today, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(route);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => sut.UpdateStatusAsync(new UpdateMailboxStatusCommand(mailbox.Id, MailboxStatus.Napunjen, userId), CancellationToken.None));

        Assert.Equal("Status je već evidentiran. Kontaktirajte dispečera za ispravku.", ex.Message);
        _auditRepo.Verify(a => a.LogAsync(It.IsAny<MailboxAuditLog>(), It.IsAny<CancellationToken>()), Times.Never);
        _repo.Verify(r => r.UpdateAsync(It.IsAny<Mailbox>(), It.IsAny<CancellationToken>()), Times.Never);
        routeRepo.Verify(r => r.UpdateAsync(It.IsAny<Route>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    // ================================================================
    // NOT FOUND
    // ================================================================

    [Fact]
    public async Task UpdateStatusAsync_ShouldThrow_WhenMailboxNotFound()
    {
        var missingId = Guid.NewGuid();

        _repo.Setup(r => r.GetByIdAsync(missingId, It.IsAny<CancellationToken>()))
             .ReturnsAsync((Mailbox?)null);

        var command = new UpdateMailboxStatusCommand(missingId, MailboxStatus.Obraen, Guid.NewGuid());
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.UpdateStatusAsync(command, CancellationToken.None));

        Assert.Contains("nije pronađen", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldNotCallAuditLog_WhenMailboxNotFound()
    {
        _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((Mailbox?)null);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.UpdateStatusAsync(new UpdateMailboxStatusCommand(Guid.NewGuid(), MailboxStatus.Obraen, Guid.NewGuid()), CancellationToken.None));

        _auditRepo.Verify(a => a.LogAsync(It.IsAny<MailboxAuditLog>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
