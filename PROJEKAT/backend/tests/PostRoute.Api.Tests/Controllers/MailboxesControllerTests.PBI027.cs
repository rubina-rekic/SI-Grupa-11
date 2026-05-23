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
/// Unit testovi za PBI-027 (US-27): PATCH /api/mailboxes/{id}/status
/// Pokriva: autorizacija, happy path, not found, invalid model
/// </summary>
public sealed class MailboxesControllerTestsPBI027
{
    private readonly Mock<IMailboxService> _serviceMock = new();
    private readonly MailboxesController _sut;

    public MailboxesControllerTestsPBI027()
    {
        _sut = new MailboxesController(_serviceMock.Object);
    }

    // ────────────────────────────────────────────────────────────────
    // Test session helper — implementira ISession bez potrebe za Moq
    // ────────────────────────────────────────────────────────────────
    private sealed class TestSession : ISession
    {
        private readonly Dictionary<string, byte[]> _data = new();

        public void SetString(string key, string value) =>
            _data[key] = Encoding.UTF8.GetBytes(value);

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

    private static Mailbox MakeMailbox(MailboxStatus status = MailboxStatus.Prazan) => new()
    {
        Id = Guid.NewGuid(),
        SerialNumber = "SN-027",
        Address = "Test adresa",
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

    private void SetSession(Guid? userId = null)
    {
        var session = new TestSession();
        if (userId.HasValue)
            session.SetString("UserId", userId.Value.ToString());

        var ctx = new DefaultHttpContext { Session = session };
        _sut.ControllerContext = new ControllerContext { HttpContext = ctx };
    }

    // ================================================================
    // HAPPY PATH
    // ================================================================

    [Fact]
    public async Task UpdateStatusAsync_ShouldReturnOk_WhenRequestIsValid()
    {
        var mailbox = MakeMailbox(MailboxStatus.Napunjen);
        var userId = Guid.NewGuid();
        SetSession(userId);

        _serviceMock
            .Setup(s => s.UpdateStatusAsync(It.IsAny<UpdateMailboxStatusCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mailbox);

        var request = new UpdateMailboxStatusRequest { Status = MailboxStatus.Napunjen };
        var result = await _sut.UpdateStatusAsync(mailbox.Id, request, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(200, ok.StatusCode);
        var response = Assert.IsType<MailboxResponse>(ok.Value);
        Assert.Equal(mailbox.Id, response.Id);
        Assert.Equal(MailboxStatus.Napunjen, response.Status);
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldPassCorrectCommandToService()
    {
        var mailbox = MakeMailbox();
        var userId = Guid.NewGuid();
        SetSession(userId);

        UpdateMailboxStatusCommand? captured = null;
        _serviceMock
            .Setup(s => s.UpdateStatusAsync(It.IsAny<UpdateMailboxStatusCommand>(), It.IsAny<CancellationToken>()))
            .Callback<UpdateMailboxStatusCommand, CancellationToken>((cmd, _) => captured = cmd)
            .ReturnsAsync(mailbox);

        var request = new UpdateMailboxStatusRequest
        {
            Status = MailboxStatus.Ispraznjen,
            Reason = "Ispražnjen tokom obilaska"
        };
        await _sut.UpdateStatusAsync(mailbox.Id, request, CancellationToken.None);

        Assert.NotNull(captured);
        Assert.Equal(mailbox.Id, captured!.MailboxId);
        Assert.Equal(MailboxStatus.Ispraznjen, captured.NewStatus);
        Assert.Equal(userId, captured.UserId);
        Assert.Equal("Ispražnjen tokom obilaska", captured.Reason);
    }

    [Theory]
    [InlineData(MailboxStatus.Obraen)]
    [InlineData(MailboxStatus.Napunjen)]
    [InlineData(MailboxStatus.Ispraznjen)]
    public async Task UpdateStatusAsync_ShouldReturnOk_ForAllPostmanStatuses(MailboxStatus status)
    {
        var mailbox = MakeMailbox(status);
        SetSession(Guid.NewGuid());

        _serviceMock
            .Setup(s => s.UpdateStatusAsync(It.IsAny<UpdateMailboxStatusCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mailbox);

        var result = await _sut.UpdateStatusAsync(mailbox.Id, new UpdateMailboxStatusRequest { Status = status }, CancellationToken.None);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    // ================================================================
    // AUTORIZACIJA — nema sesije
    // ================================================================

    [Fact]
    public async Task UpdateStatusAsync_ShouldReturnUnauthorized_WhenSessionHasNoUserId()
    {
        SetSession(userId: null);  // sesija postoji ali nema UserId

        var result = await _sut.UpdateStatusAsync(Guid.NewGuid(), new UpdateMailboxStatusRequest { Status = MailboxStatus.Obraen }, CancellationToken.None);

        Assert.IsType<UnauthorizedObjectResult>(result.Result);
        _serviceMock.Verify(s => s.UpdateStatusAsync(It.IsAny<UpdateMailboxStatusCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldReturnUnauthorized_WhenSessionUserIdIsNotValidGuid()
    {
        var session = new TestSession();
        session.SetString("UserId", "nije-validan-guid");
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { Session = session }
        };

        var result = await _sut.UpdateStatusAsync(Guid.NewGuid(), new UpdateMailboxStatusRequest { Status = MailboxStatus.Obraen }, CancellationToken.None);

        Assert.IsType<UnauthorizedObjectResult>(result.Result);
        _serviceMock.Verify(s => s.UpdateStatusAsync(It.IsAny<UpdateMailboxStatusCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    // ================================================================
    // NOT FOUND
    // ================================================================

    [Fact]
    public async Task UpdateStatusAsync_ShouldReturnNotFound_WhenMailboxDoesNotExist()
    {
        SetSession(Guid.NewGuid());

        _serviceMock
            .Setup(s => s.UpdateStatusAsync(It.IsAny<UpdateMailboxStatusCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Sandučić nije pronađen."));

        var result = await _sut.UpdateStatusAsync(Guid.NewGuid(), new UpdateMailboxStatusRequest { Status = MailboxStatus.Obraen }, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    // ================================================================
    // VALIDACIJA MODELA
    // ================================================================

    [Fact]
    public async Task UpdateStatusAsync_ShouldReturnBadRequest_WhenModelStateInvalid()
    {
        SetSession(Guid.NewGuid());
        _sut.ModelState.AddModelError("Status", "Status je obavezan");

        var result = await _sut.UpdateStatusAsync(Guid.NewGuid(), new UpdateMailboxStatusRequest(), CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
        _serviceMock.Verify(s => s.UpdateStatusAsync(It.IsAny<UpdateMailboxStatusCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
