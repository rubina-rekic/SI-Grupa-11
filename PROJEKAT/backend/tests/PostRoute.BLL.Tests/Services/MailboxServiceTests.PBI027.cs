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
