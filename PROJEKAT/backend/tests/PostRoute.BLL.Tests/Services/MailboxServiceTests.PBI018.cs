using Moq;
using PostRoute.BLL.Commands;
using PostRoute.BLL.Services;
using PostRoute.DAL.Entities;
using PostRoute.DAL.Repositories;

namespace PostRoute.BLL.Tests.Services;

public sealed class MailboxServiceTestsPBI018
{
    private readonly Mock<IMailboxRepository> _repo;
    private readonly Mock<IMailboxAuditLogRepository> _auditRepo;
    private readonly MailboxService _sut;

    public MailboxServiceTestsPBI018()
    {
        _repo = new Mock<IMailboxRepository>();
        _auditRepo = new Mock<IMailboxAuditLogRepository>();
        _sut = new MailboxService(_repo.Object, _auditRepo.Object);
    }

    private static Mailbox ExistingMailbox(Guid id) => new()
    {
        Id = id,
        SerialNumber = "OLD-001",
        Address = "Stara adresa",
        Latitude = 43.85m,
        Longitude = 18.41m,
        Type = MailboxType.WallSmall,
        Priority = MailboxPriority.Srednji,
        Status = MailboxStatus.Prazan,
        Capacity = 100,
        InstallationYear = 2020,
        Notes = "stare napomene"
    };

    private static UpdateMailboxCommand FullCommand(Guid id, Guid userId) => new(
        Id: id,
        SerialNumber: "NEW-001",
        Address: "Nova adresa",
        Latitude: 44.0m,
        Longitude: 18.0m,
        Type: MailboxType.SpecialPriority,
        Priority: MailboxPriority.Visok,
        Capacity: 200,
        InstallationYear: 2025,
        Notes: "nove napomene",
        UserId: userId
    );

    [Fact]
    public async Task UpdateAsync_ThrowsWhenMailboxNotFound()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((Mailbox?)null);

        var cmd = FullCommand(id, Guid.NewGuid());
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.UpdateAsync(cmd, CancellationToken.None));
        Assert.Contains("Sandučić nije pronađen", ex.Message);
    }

    [Theory]
    [InlineData(-91)]
    [InlineData(91)]
    public async Task UpdateAsync_ThrowsWhenLatitudeOutOfRange(decimal lat)
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(ExistingMailbox(id));
        var cmd = FullCommand(id, Guid.NewGuid()) with { Latitude = lat };
        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.UpdateAsync(cmd, CancellationToken.None));
    }

    [Theory]
    [InlineData(-181)]
    [InlineData(181)]
    public async Task UpdateAsync_ThrowsWhenLongitudeOutOfRange(decimal lng)
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(ExistingMailbox(id));
        var cmd = FullCommand(id, Guid.NewGuid()) with { Longitude = lng };
        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.UpdateAsync(cmd, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateAsync_ThrowsWhenCapacityNotPositive()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(ExistingMailbox(id));
        var cmd = FullCommand(id, Guid.NewGuid()) with { Capacity = 0 };
        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.UpdateAsync(cmd, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateAsync_ThrowsWhenInstallationYearOutOfRange()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(ExistingMailbox(id));
        var cmd = FullCommand(id, Guid.NewGuid()) with { InstallationYear = 1800 };
        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.UpdateAsync(cmd, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateAsync_PersistsAllFieldsToRepository()
    {
        var id = Guid.NewGuid();
        var existing = ExistingMailbox(id);
        _repo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        Mailbox? captured = null;
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Mailbox>(), It.IsAny<CancellationToken>()))
             .Callback<Mailbox, CancellationToken>((m, _) => captured = m)
             .ReturnsAsync((Mailbox m, CancellationToken _) => m);

        var cmd = FullCommand(id, Guid.NewGuid());
        await _sut.UpdateAsync(cmd, CancellationToken.None);

        Assert.NotNull(captured);
        Assert.Equal("NEW-001", captured!.SerialNumber);
        Assert.Equal("Nova adresa", captured.Address);
        Assert.Equal(44.0m, captured.Latitude);
        Assert.Equal(18.0m, captured.Longitude);
        Assert.Equal(MailboxType.SpecialPriority, captured.Type);
        Assert.Equal(MailboxPriority.Visok, captured.Priority);
        Assert.Equal(200, captured.Capacity);
        Assert.Equal(2025, captured.InstallationYear);
        Assert.Equal("nove napomene", captured.Notes);
    }

    [Fact]
    public async Task UpdateAsync_LogsAuditEntryForEachChangedField()
    {
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(ExistingMailbox(id));
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Mailbox>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((Mailbox m, CancellationToken _) => m);

        var capturedLogs = new List<MailboxAuditLog>();
        _auditRepo.Setup(a => a.LogAsync(It.IsAny<MailboxAuditLog>(), It.IsAny<CancellationToken>()))
                  .Callback<MailboxAuditLog, CancellationToken>((log, _) => capturedLogs.Add(log));

        var cmd = FullCommand(id, userId);
        await _sut.UpdateAsync(cmd, CancellationToken.None);

        Assert.Equal(9, capturedLogs.Count);
        Assert.All(capturedLogs, log =>
        {
            Assert.Equal(id, log.MailboxId);
            Assert.Equal(userId, log.UserId);
            Assert.Equal("UPDATE", log.Action);
            Assert.NotNull(log.NewValue);
        });
        Assert.Contains(capturedLogs, l => l.FieldName == "SerialNumber" && l.OldValue == "OLD-001" && l.NewValue == "NEW-001");
    }

    [Fact]
    public async Task UpdateAsync_DoesNotLogAuditForUnchangedFields()
    {
        var id = Guid.NewGuid();
        var existing = ExistingMailbox(id);
        _repo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Mailbox>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((Mailbox m, CancellationToken _) => m);

        var cmd = new UpdateMailboxCommand(
            Id: id,
            SerialNumber: existing.SerialNumber,
            Address: existing.Address,
            Latitude: existing.Latitude,
            Longitude: existing.Longitude,
            Type: existing.Type,
            Priority: existing.Priority,
            Capacity: existing.Capacity,
            InstallationYear: existing.InstallationYear,
            Notes: existing.Notes,
            UserId: Guid.NewGuid()
        );

        await _sut.UpdateAsync(cmd, CancellationToken.None);

        _auditRepo.Verify(a => a.LogAsync(It.IsAny<MailboxAuditLog>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
