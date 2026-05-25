using Moq;
using PostRoute.BLL.Commands;
using PostRoute.BLL.Services;
using PostRoute.DAL.Entities;
using PostRoute.DAL.Repositories;
using Xunit;

namespace PostRoute.BLL.Tests.Services;

/// <summary>
/// Unit testovi za PBI-028 (US-28): Označavanje nedostupne lokacije
/// Pokriva: UpdateStatusAsync sa Nedostupan statusom i razlogom nedostupnosti
/// </summary>
public sealed class MailboxServiceTestsPBI028
{
    private readonly Mock<IMailboxRepository> _repo;
    private readonly Mock<IMailboxAuditLogRepository> _auditRepo;
    private readonly MailboxService _sut;

    public MailboxServiceTestsPBI028()
    {
        _repo = new Mock<IMailboxRepository>();
        _auditRepo = new Mock<IMailboxAuditLogRepository>();
        _sut = new MailboxService(_repo.Object, _auditRepo.Object);
    }

    private static Mailbox MakeMailbox() => new()
    {
        Id = Guid.NewGuid(),
        SerialNumber = "SN-028",
        Address = "Testna adresa 28",
        Latitude = 43.85m,
        Longitude = 18.41m,
        Type = MailboxType.WallSmall,
        Priority = MailboxPriority.Srednji,
        Status = MailboxStatus.Prazan,
        Capacity = 100,
        InstallationYear = 2024,
        WorkingDays = MailboxWorkingDays.RadniDani,
        IsAlwaysAvailable = true
    };

    [Fact]
    public async Task UpdateStatusAsync_ShouldSetNedostupan_WhenLocationUnavailable()
    {
        var mailbox = MakeMailbox();
        var userId = Guid.NewGuid();
        var command = new UpdateMailboxStatusCommand(mailbox.Id, MailboxStatus.Nedostupan, userId, "Zaključan pristup");

        _repo.Setup(r => r.GetByIdAsync(mailbox.Id, It.IsAny<CancellationToken>())).ReturnsAsync(mailbox);
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Mailbox>(), It.IsAny<CancellationToken>())).ReturnsAsync((Mailbox m, CancellationToken _) => m);
        _auditRepo.Setup(a => a.LogAsync(It.IsAny<MailboxAuditLog>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _sut.UpdateStatusAsync(command, CancellationToken.None);

        Assert.Equal(MailboxStatus.Nedostupan, result.Status);
    }

    [Theory]
    [InlineData("Zaključan pristup")]
    [InlineData("Sandučić oštećen")]
    [InlineData("Privatni posjed nedostupan")]
    [InlineData("Prirodna prepreka")]
    [InlineData("Ostalo — nema pristupa")]
    public async Task UpdateStatusAsync_ShouldAcceptAllUnavailableReasons(string reason)
    {
        var mailbox = MakeMailbox();
        var userId = Guid.NewGuid();
        var command = new UpdateMailboxStatusCommand(mailbox.Id, MailboxStatus.Nedostupan, userId, reason);

        _repo.Setup(r => r.GetByIdAsync(mailbox.Id, It.IsAny<CancellationToken>())).ReturnsAsync(mailbox);
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Mailbox>(), It.IsAny<CancellationToken>())).ReturnsAsync((Mailbox m, CancellationToken _) => m);
        _auditRepo.Setup(a => a.LogAsync(It.IsAny<MailboxAuditLog>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _sut.UpdateStatusAsync(command, CancellationToken.None);

        Assert.Equal(MailboxStatus.Nedostupan, result.Status);
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldLogReason_InAuditLog_WhenUnavailable()
    {
        var mailbox = MakeMailbox();
        var userId = Guid.NewGuid();
        const string reason = "Zaključan pristup";
        var command = new UpdateMailboxStatusCommand(mailbox.Id, MailboxStatus.Nedostupan, userId, reason);

        MailboxAuditLog? capturedLog = null;
        _repo.Setup(r => r.GetByIdAsync(mailbox.Id, It.IsAny<CancellationToken>())).ReturnsAsync(mailbox);
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Mailbox>(), It.IsAny<CancellationToken>())).ReturnsAsync((Mailbox m, CancellationToken _) => m);
        _auditRepo.Setup(a => a.LogAsync(It.IsAny<MailboxAuditLog>(), It.IsAny<CancellationToken>()))
            .Callback<MailboxAuditLog, CancellationToken>((log, _) => capturedLog = log)
            .Returns(Task.CompletedTask);

        await _sut.UpdateStatusAsync(command, CancellationToken.None);

        Assert.NotNull(capturedLog);
        Assert.Equal(reason, capturedLog.Reason);
        Assert.Equal(userId, capturedLog.UserId);
        Assert.Equal("Status", capturedLog.FieldName);
        Assert.Equal("Prazan", capturedLog.OldValue);
        Assert.Equal("Nedostupan", capturedLog.NewValue);
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldThrow_WhenMailboxNotFound_ForUnavailable()
    {
        var nonExistentId = Guid.NewGuid();
        var command = new UpdateMailboxStatusCommand(nonExistentId, MailboxStatus.Nedostupan, Guid.NewGuid(), "Zaključan pristup");

        _repo.Setup(r => r.GetByIdAsync(nonExistentId, It.IsAny<CancellationToken>())).ReturnsAsync((Mailbox?)null);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.UpdateStatusAsync(command, CancellationToken.None));

        _auditRepo.Verify(a => a.LogAsync(It.IsAny<MailboxAuditLog>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
