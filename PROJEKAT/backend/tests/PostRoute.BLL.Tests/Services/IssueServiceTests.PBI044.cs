using Moq;
using PostRoute.BLL.Models.Issues;
using PostRoute.BLL.Services;
using PostRoute.DAL.Entities;
using PostRoute.DAL.Repositories;
using Xunit;

namespace PostRoute.BLL.Tests.Services;


public sealed class IssueServiceTestsPBI044
{
    private readonly Mock<IIssueRepository> _repo;
    private readonly IssueService _sut;

    public IssueServiceTestsPBI044()
    {
        _repo = new Mock<IIssueRepository>();
        _sut = new IssueService(_repo.Object);
    }

    private static IssueNotification MakeNotification(Guid recipientUserId, bool isRead = false)
        => new()
        {
            Id = Guid.NewGuid(),
            IssueId = Guid.NewGuid(),
            RecipientUserId = recipientUserId,
            Title = "Test notifikacija",
            Message = "Testna poruka notifikacije.",
            IsRead = isRead,
            CreatedAt = DateTime.UtcNow,
            Issue = new Issue
            {
                Id = Guid.NewGuid(),
                Mailbox = new Mailbox { Address = "Testna adresa 123" }
            },
            Recipient = new User { Id = recipientUserId, Username = "korisnik" }
        };

    [Fact]
    public async Task GetMyNotificationsAsync_ShouldReturnMappedNotifications_WhenNotificationsExist()
    {
        var userId = Guid.NewGuid();
        var notification = MakeNotification(userId);
        _repo.Setup(r => r.GetTodayNotificationsAsync(userId, It.IsAny<CancellationToken>()))
             .ReturnsAsync(new[] { notification });

        var result = await _sut.GetMyNotificationsAsync(userId, CancellationToken.None);

        var list = Assert.Single(result);
        Assert.Equal(notification.Id, list.Id);
        Assert.Equal(notification.IssueId, list.IssueId);
        Assert.Equal(notification.Issue.Mailbox.Address, list.MailboxAddress);
        Assert.Equal(notification.Title, list.Title);
        Assert.Equal(notification.Message, list.Message);
        Assert.Equal(notification.IsRead, list.IsRead);
        Assert.Equal(notification.CreatedAt, list.CreatedAt);
    }

    [Fact]
    public async Task GetMyNotificationsAsync_ShouldReturnEmptyList_WhenNoNotificationsExist()
    {
        var userId = Guid.NewGuid();
        _repo.Setup(r => r.GetTodayNotificationsAsync(userId, It.IsAny<CancellationToken>()))
             .ReturnsAsync(Array.Empty<IssueNotification>());

        var result = await _sut.GetMyNotificationsAsync(userId, CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task MarkNotificationReadAsync_ShouldSetIsReadTrue_WhenRecipientMatches()
    {
        var userId = Guid.NewGuid();
        var notification = MakeNotification(userId, isRead: false);

        _repo.Setup(r => r.GetNotificationByIdAsync(notification.Id, It.IsAny<CancellationToken>()))
             .ReturnsAsync(notification);
        _repo.Setup(r => r.UpdateNotificationAsync(It.IsAny<IssueNotification>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((IssueNotification n, CancellationToken _) => n);

        await _sut.MarkNotificationReadAsync(notification.Id, userId, CancellationToken.None);

        Assert.True(notification.IsRead);
        _repo.Verify(r => r.UpdateNotificationAsync(It.Is<IssueNotification>(n => n.Id == notification.Id && n.IsRead), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MarkNotificationReadAsync_ShouldThrow_WhenNotificationNotFound()
    {
        var userId = Guid.NewGuid();
        _repo.Setup(r => r.GetNotificationByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((IssueNotification?)null);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.MarkNotificationReadAsync(Guid.NewGuid(), userId, CancellationToken.None));
    }

    [Fact]
    public async Task MarkNotificationReadAsync_ShouldThrow_WhenRecipientDoesNotMatch()
    {
        var userId = Guid.NewGuid();
        var notification = MakeNotification(Guid.NewGuid());

        _repo.Setup(r => r.GetNotificationByIdAsync(notification.Id, It.IsAny<CancellationToken>()))
             .ReturnsAsync(notification);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.MarkNotificationReadAsync(notification.Id, userId, CancellationToken.None));
    }
}
