using System.Linq;
using Moq;
using PostRoute.BLL.Services;
using PostRoute.DAL.Entities;
using PostRoute.DAL.Repositories;
using Xunit;

namespace PostRoute.BLL.Tests.Services;


public sealed class IssueServiceTestsPBI052
{
    private readonly Mock<IIssueRepository> _repo;
    private readonly IssueService _sut;

    public IssueServiceTestsPBI052()
    {
        _repo = new Mock<IIssueRepository>();
        _sut = new IssueService(_repo.Object);
    }

    private static User MakeUser(Guid id, string username) => new()
    {
        Id = id,
        Username = username,
        Role = "Dispatcher"
    };

    private static Issue MakeIssue(Guid id, Guid reporterId) => new()
    {
        Id = id,
        RouteItemId = Guid.NewGuid(),
        RouteItem = new RouteItem(),
        MailboxId = Guid.NewGuid(),
        Mailbox = new Mailbox { Address = "Testna adresa" },
        ReportedByUserId = reporterId,
        ReportedBy = MakeUser(reporterId, "reporter"),
        Status = IssueStatus.Otvoren,
        CreatedAt = DateTime.UtcNow.AddMinutes(-5),
        UpdatedAt = DateTime.UtcNow.AddMinutes(-5)
    };

    [Fact]
    public async Task GetByIdAsync_ShouldReturnMappedIssue_WhenIssueExists()
    {
        var issueId = Guid.NewGuid();
        var issue = MakeIssue(issueId, Guid.NewGuid());

        _repo.Setup(r => r.GetByIdAsync(issueId, It.IsAny<CancellationToken>()))
             .ReturnsAsync(issue);

        var result = await _sut.GetByIdAsync(issueId, CancellationToken.None);

        Assert.Equal(issueId, result.Id);
        Assert.Equal(issue.Mailbox.Address, result.MailboxAddress);
        Assert.Equal("Otvoren", result.StatusLabel);
        Assert.Equal(issue.ReportedBy.Username, result.ReportedByUsername);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrow_WhenIssueNotFound()
    {
        var issueId = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(issueId, It.IsAny<CancellationToken>()))
             .ReturnsAsync((Issue?)null);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.GetByIdAsync(issueId, CancellationToken.None));
    }

    [Fact]
    public async Task AddCommentAsync_ShouldAddCommentAndSetStatusToInProgress_WhenIssueOpen()
    {
        var issueId = Guid.NewGuid();
        var reporterId = Guid.NewGuid();
        var issue = MakeIssue(issueId, reporterId);
        var authorId = Guid.NewGuid();

        _repo.Setup(r => r.GetByIdAsync(issueId, It.IsAny<CancellationToken>()))
             .ReturnsAsync(issue);
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Issue>(), It.IsAny<CancellationToken>()))
             .Callback<Issue, CancellationToken>((i, _) =>
             {
                 var comment = i.Comments.Last();
                 comment.Author = MakeUser(authorId, "dispatcher");
             })
             .ReturnsAsync((Issue i, CancellationToken _) => i);

        var result = await _sut.AddCommentAsync(issueId, authorId, "Dispatcher", "Komentar", CancellationToken.None);

        Assert.Equal(IssueStatus.UObradi, result.Status);
        Assert.Single(result.Comments);
        Assert.Equal("Komentar", result.Comments[0].Content);
        _repo.Verify(r => r.UpdateAsync(It.Is<Issue>(i => i.Status == IssueStatus.UObradi && i.Comments.Count == 1), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddCommentAsync_ShouldCreateNotification_WhenDispatcherAddsComment()
    {
        var issueId = Guid.NewGuid();
        var reporterId = Guid.NewGuid();
        var issue = MakeIssue(issueId, reporterId);
        var authorId = Guid.NewGuid();

        _repo.Setup(r => r.GetByIdAsync(issueId, It.IsAny<CancellationToken>()))
             .ReturnsAsync(issue);
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Issue>(), It.IsAny<CancellationToken>()))
             .Callback<Issue, CancellationToken>((i, _) =>
             {
                 var comment = i.Comments.Last();
                 comment.Author = MakeUser(authorId, "dispatcher");
             })
             .ReturnsAsync((Issue i, CancellationToken _) => i);

        await _sut.AddCommentAsync(issueId, authorId, "Dispatcher", "Dispatcher komentar", CancellationToken.None);

        Assert.Single(issue.Notifications);
        Assert.Equal(reporterId, issue.Notifications.First().RecipientUserId);
        Assert.Contains("Dispatcher komentar", issue.Notifications.First().Message);
    }

    [Fact]
    public async Task AssignActionAsync_ShouldAssignActionAndSetAssignedTo_WhenActionIsDrugiPostar()
    {
        var issueId = Guid.NewGuid();
        var dispatcherId = Guid.NewGuid();
        var targetPostmanId = Guid.NewGuid();
        var issue = MakeIssue(issueId, Guid.NewGuid());

        _repo.Setup(r => r.GetByIdAsync(issueId, It.IsAny<CancellationToken>()))
             .ReturnsAsync(issue);
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Issue>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((Issue i, CancellationToken _) => i);

        var result = await _sut.AssignActionAsync(issueId, dispatcherId, IssueAction.DrugiPostar, targetPostmanId, CancellationToken.None);

        Assert.Equal(IssueAction.DrugiPostar, result.AssignedAction);
        Assert.Equal(targetPostmanId, result.ActionAssignedToUserId);
        Assert.Equal(IssueStatus.UObradi, result.Status);
        _repo.Verify(r => r.UpdateAsync(It.Is<Issue>(i => i.AssignedAction == IssueAction.DrugiPostar && i.ActionAssignedToUserId == targetPostmanId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ResolveAsync_ShouldMarkIssueAsResolved_WhenIssueIsNotResolved()
    {
        var issueId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var issue = MakeIssue(issueId, Guid.NewGuid());

        _repo.Setup(r => r.GetByIdAsync(issueId, It.IsAny<CancellationToken>()))
             .ReturnsAsync(issue);
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Issue>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((Issue i, CancellationToken _) => i);

        var result = await _sut.ResolveAsync(issueId, userId, CancellationToken.None);

        Assert.Equal(IssueStatus.Rijesen, result.Status);
        _repo.Verify(r => r.UpdateAsync(It.Is<Issue>(i => i.Status == IssueStatus.Rijesen), It.IsAny<CancellationToken>()), Times.Once);
    }
}
