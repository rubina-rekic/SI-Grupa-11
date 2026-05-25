using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PostRoute.Api.Contracts.Mailboxes;
using PostRoute.Api.Controllers;
using PostRoute.BLL.Commands;
using PostRoute.BLL.Services;
using PostRoute.DAL.Entities;
using System.Text;
using Xunit;

namespace PostRoute.Api.Tests.Controllers;

/// <summary>
/// Unit testovi za PBI-028 (US-28): PATCH /api/mailboxes/{id}/status sa Nedostupan statusom
/// Pokriva: autorizacija, happy path sa razlogom, validacija razloga
/// </summary>
public sealed class MailboxesControllerTestsPBI028
{
    private readonly Mock<IMailboxService> _serviceMock = new();
    private readonly MailboxesController _sut;

    public MailboxesControllerTestsPBI028()
    {
        _sut = new MailboxesController(_serviceMock.Object);
    }

    private sealed class TestSession : ISession
    {
        private readonly Dictionary<string, byte[]> _data = new();
        public void SetString(string key, string value) => _data[key] = Encoding.UTF8.GetBytes(value);
        public bool TryGetValue(string key, out byte[] value)
        {
            var found = _data.TryGetValue(key, out var bytes);
            value = bytes ?? [];
            return found;
        }
        public string Id => "test-session";
        public bool IsAvailable => true;
        public IEnumerable<string> Keys => _data.Keys;
        public void Clear() => _data.Clear();
        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Remove(string key) => _data.Remove(key);
        public void Set(string key, byte[] value) => _data[key] = value;
    }

    private static Mailbox MakeMailbox() => new()
    {
        Id = Guid.NewGuid(),
        SerialNumber = "SN-028",
        Address = "Test adresa 28",
        Latitude = 43.85m,
        Longitude = 18.41m,
        Type = MailboxType.WallSmall,
        Priority = MailboxPriority.Srednji,
        Status = MailboxStatus.Nedostupan,
        Capacity = 100,
        InstallationYear = 2024,
        WorkingDays = MailboxWorkingDays.RadniDani,
        IsAlwaysAvailable = true
    };

    private HttpContext MakeHttpContext(string? userId)
    {
        var session = new TestSession();
        if (userId is not null) session.SetString("UserId", userId);
        var ctx = new DefaultHttpContext { Session = session };
        return ctx;
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldReturnOk_WhenNedostupanWithReason()
    {
        var mailbox = MakeMailbox();
        var userId = Guid.NewGuid().ToString();
        var request = new UpdateMailboxStatusRequest { Status = MailboxStatus.Nedostupan, Reason = "Zaključan pristup" };

        _sut.ControllerContext = new ControllerContext { HttpContext = MakeHttpContext(userId) };
        _serviceMock.Setup(s => s.UpdateStatusAsync(It.IsAny<UpdateMailboxStatusCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(mailbox);

        var result = await _sut.UpdateStatusAsync(mailbox.Id, request, CancellationToken.None);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldPassReason_ToService_WhenUnavailable()
    {
        var mailbox = MakeMailbox();
        var userId = Guid.NewGuid().ToString();
        const string reason = "Sandučić oštećen";
        var request = new UpdateMailboxStatusRequest { Status = MailboxStatus.Nedostupan, Reason = reason };

        UpdateMailboxStatusCommand? capturedCommand = null;
        _sut.ControllerContext = new ControllerContext { HttpContext = MakeHttpContext(userId) };
        _serviceMock.Setup(s => s.UpdateStatusAsync(It.IsAny<UpdateMailboxStatusCommand>(), It.IsAny<CancellationToken>()))
            .Callback<UpdateMailboxStatusCommand, CancellationToken>((cmd, _) => capturedCommand = cmd)
            .ReturnsAsync(mailbox);

        await _sut.UpdateStatusAsync(mailbox.Id, request, CancellationToken.None);

        Assert.NotNull(capturedCommand);
        Assert.Equal(MailboxStatus.Nedostupan, capturedCommand.NewStatus);
        Assert.Equal(reason, capturedCommand.Reason);
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldReturnUnauthorized_WhenNoSession_ForUnavailable()
    {
        var mailbox = MakeMailbox();
        var request = new UpdateMailboxStatusRequest { Status = MailboxStatus.Nedostupan, Reason = "Zaključan pristup" };

        _sut.ControllerContext = new ControllerContext { HttpContext = MakeHttpContext(null) };

        var result = await _sut.UpdateStatusAsync(mailbox.Id, request, CancellationToken.None);

        Assert.True(result.Result is UnauthorizedResult or UnauthorizedObjectResult);
        _serviceMock.Verify(s => s.UpdateStatusAsync(It.IsAny<UpdateMailboxStatusCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldReturnNotFound_WhenMailboxMissing_ForUnavailable()
    {
        var mailboxId = Guid.NewGuid();
        var userId = Guid.NewGuid().ToString();
        var request = new UpdateMailboxStatusRequest { Status = MailboxStatus.Nedostupan, Reason = "Prirodna prepreka" };

        _sut.ControllerContext = new ControllerContext { HttpContext = MakeHttpContext(userId) };
        _serviceMock.Setup(s => s.UpdateStatusAsync(It.IsAny<UpdateMailboxStatusCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Sandučić nije pronađen"));

        var result = await _sut.UpdateStatusAsync(mailboxId, request, CancellationToken.None);

        Assert.True(result.Result is NotFoundResult or NotFoundObjectResult);
    }
}
