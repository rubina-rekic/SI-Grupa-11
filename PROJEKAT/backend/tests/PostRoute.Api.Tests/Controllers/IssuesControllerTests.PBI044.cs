using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PostRoute.Api.Controllers;
using PostRoute.BLL.Models.Issues;
using PostRoute.BLL.Services;
using PostRoute.Domain.Entities;
using Xunit;

namespace PostRoute.Api.Tests.Controllers;

public sealed class IssuesControllerTestsPBI044
{
    private readonly Mock<IIssueService> _serviceMock = new();
    private readonly IssuesController _sut;

    public IssuesControllerTestsPBI044()
    {
        _sut = new IssuesController(_serviceMock.Object);
    }

    private void SetUser(Guid? userId, string? role = null)
    {
        var claims = new List<Claim>();
        if (userId.HasValue)
            claims.Add(new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString()));
        if (!string.IsNullOrWhiteSpace(role))
            claims.Add(new Claim(ClaimTypes.Role, role));

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    [Fact]
    public async Task GetMyNotifications_ShouldReturnOk_WithNotificationList()
    {
        var userId = Guid.NewGuid();
        SetUser(userId, UserRole.PostalWorker);

        var notifications = new[]
        {
            new IssueNotificationModel(Guid.NewGuid(), Guid.NewGuid(), "Adresa 1", "Naslov 1", "Poruka 1", false, DateTime.UtcNow)
        };

        _serviceMock.Setup(s => s.GetMyNotificationsAsync(userId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(notifications);

        var result = await _sut.GetMyNotifications(CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsAssignableFrom<IEnumerable<IssueNotificationModel>>(ok.Value);
        Assert.Single(response);
    }

    [Fact]
    public async Task GetMyNotifications_ShouldReturnUnauthorized_WhenUserIdMissing()
    {
        SetUser(userId: null, role: UserRole.PostalWorker);

        var result = await _sut.GetMyNotifications(CancellationToken.None);

        Assert.IsType<UnauthorizedObjectResult>(result);
        _serviceMock.Verify(s => s.GetMyNotificationsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task MarkNotificationRead_ShouldReturnOk_WhenServiceSucceeds()
    {
        var userId = Guid.NewGuid();
        SetUser(userId, UserRole.PostalWorker);

        _serviceMock.Setup(s => s.MarkNotificationReadAsync(It.IsAny<Guid>(), userId, It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

        var result = await _sut.MarkNotificationRead(Guid.NewGuid(), CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, ok.StatusCode);
    }

    [Fact]
    public async Task MarkNotificationRead_ShouldReturnBadRequest_WhenServiceThrowsInvalidOperationException()
    {
        var userId = Guid.NewGuid();
        SetUser(userId, UserRole.PostalWorker);

        _serviceMock.Setup(s => s.MarkNotificationReadAsync(It.IsAny<Guid>(), userId, It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new InvalidOperationException("Notifikacija nije pronađena."));

        var result = await _sut.MarkNotificationRead(Guid.NewGuid(), CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task MarkNotificationRead_ShouldReturnUnauthorized_WhenUserIdInvalidGuid()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "nije-validan-guid"),
            new Claim(ClaimTypes.Role, UserRole.PostalWorker)
        };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        var result = await _sut.MarkNotificationRead(Guid.NewGuid(), CancellationToken.None);

        Assert.IsType<UnauthorizedObjectResult>(result);
        _serviceMock.Verify(s => s.MarkNotificationReadAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
