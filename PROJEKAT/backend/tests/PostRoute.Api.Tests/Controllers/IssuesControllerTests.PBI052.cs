using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PostRoute.Api.Controllers;
using PostRoute.BLL.Models.Issues;
using PostRoute.BLL.Services;
using PostRoute.DAL.Entities;
using PostRoute.Domain.Entities;
using Xunit;

namespace PostRoute.Api.Tests.Controllers;


public sealed class IssuesControllerTestsPBI052
{
    private readonly Mock<IIssueService> _serviceMock = new();
    private readonly IssuesController _sut;

    public IssuesControllerTestsPBI052()
    {
        _sut = new IssuesController(_serviceMock.Object);
    }

    private void SetUser(Guid userId, string role)
    {
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, role)
        }, "TestAuth");

        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
        };
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_WhenIssueExists()
    {
        var issueId = Guid.NewGuid();
        var issue = new IssueModel(
            issueId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Adresa 1",
            "SN-1",
            Guid.NewGuid(),
            "reporter",
            "Nije dostupno",
            IssueStatus.Otvoren,
            "Otvoren",
            null,
            null,
            null,
            null,
            DateTime.UtcNow.AddMinutes(-10),
            DateTime.UtcNow,
            new List<IssueCommentModel>(),
            new List<IssueTimelineEntryModel>());

        _serviceMock.Setup(s => s.GetByIdAsync(issueId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(issue);
        SetUser(Guid.NewGuid(), UserRole.Dispatcher);

        var result = await _sut.GetById(issueId, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(issueId, Assert.IsType<IssueModel>(ok.Value).Id);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenIssueDoesNotExist()
    {
        var issueId = Guid.NewGuid();
        _serviceMock.Setup(s => s.GetByIdAsync(issueId, It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new InvalidOperationException("Problem nije pronađen."));
        SetUser(Guid.NewGuid(), UserRole.Dispatcher);

        var result = await _sut.GetById(issueId, CancellationToken.None);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task AddComment_ShouldReturnOk_WhenCommentAdded()
    {
        var issueId = Guid.NewGuid();
        var expectedIssue = new IssueModel(
            issueId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Adresa 2",
            "SN-2",
            Guid.NewGuid(),
            "reporter",
            null,
            IssueStatus.UObradi,
            "U obradi",
            null,
            null,
            null,
            null,
            DateTime.UtcNow.AddMinutes(-20),
            DateTime.UtcNow,
            new List<IssueCommentModel> { new IssueCommentModel(Guid.NewGuid(), Guid.NewGuid(), "dispatcher", UserRole.Dispatcher, "OK", DateTime.UtcNow) },
            new List<IssueTimelineEntryModel>());

        _serviceMock.Setup(s => s.AddCommentAsync(issueId, It.IsAny<Guid>(), It.IsAny<string>(), "Komentar", It.IsAny<CancellationToken>()))
                    .ReturnsAsync(expectedIssue);
        SetUser(Guid.NewGuid(), UserRole.Dispatcher);

        var result = await _sut.AddComment(issueId, new AddCommentRequest("Komentar"), CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<IssueModel>(ok.Value);
        Assert.Equal(issueId, response.Id);
        Assert.Single(response.Comments);
    }

    [Fact]
    public async Task AssignAction_ShouldReturnOk_WhenActionAssigned()
    {
        var issueId = Guid.NewGuid();
        var expectedIssue = new IssueModel(
            issueId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Adresa 3",
            "SN-3",
            Guid.NewGuid(),
            "reporter",
            null,
            IssueStatus.UObradi,
            "U obradi",
            IssueAction.DrugiPostar,
            "Dodijeli drugom poštaru",
            Guid.NewGuid(),
            "postar",
            DateTime.UtcNow.AddMinutes(-30),
            DateTime.UtcNow,
            new List<IssueCommentModel>(),
            new List<IssueTimelineEntryModel>());

        _serviceMock.Setup(s => s.AssignActionAsync(issueId, It.IsAny<Guid>(), IssueAction.DrugiPostar, It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(expectedIssue);
        SetUser(Guid.NewGuid(), UserRole.Dispatcher);

        var request = new AssignActionRequest(IssueAction.DrugiPostar, Guid.NewGuid());
        var result = await _sut.AssignAction(issueId, request, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<IssueModel>(ok.Value);
        Assert.Equal(IssueAction.DrugiPostar, response.AssignedAction);
    }

    [Fact]
    public async Task Resolve_ShouldReturnOk_WhenIssueResolved()
    {
        var issueId = Guid.NewGuid();
        var expectedIssue = new IssueModel(
            issueId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Adresa 4",
            "SN-4",
            Guid.NewGuid(),
            "reporter",
            null,
            IssueStatus.Rijesen,
            "Riješen",
            null,
            null,
            null,
            null,
            DateTime.UtcNow.AddMinutes(-40),
            DateTime.UtcNow,
            new List<IssueCommentModel>(),
            new List<IssueTimelineEntryModel>());

        _serviceMock.Setup(s => s.ResolveAsync(issueId, It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(expectedIssue);
        SetUser(Guid.NewGuid(), UserRole.Dispatcher);

        var result = await _sut.Resolve(issueId, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<IssueModel>(ok.Value);
        Assert.Equal(IssueStatus.Rijesen, response.Status);
    }
}
