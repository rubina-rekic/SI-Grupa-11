using Moq;
using PostRoute.BLL.Commands;
using PostRoute.BLL.Services;
using PostRoute.DAL.Entities;
using PostRoute.DAL.Repositories;

namespace PostRoute.BLL.Tests.Services;

public sealed class MailboxServiceTestsPBI019
{
    private readonly Mock<IMailboxRepository> _repo;
    private readonly Mock<IMailboxAuditLogRepository> _auditLogRepo;
    private readonly MailboxService _sut;

    public MailboxServiceTestsPBI019()
    {
        _repo = new Mock<IMailboxRepository>();
        _auditLogRepo = new Mock<IMailboxAuditLogRepository>();
        _sut = new MailboxService(_repo.Object, _auditLogRepo.Object);
    }

    private static Mailbox MakeMailbox(string sn = "SN1", MailboxPriority p = MailboxPriority.Srednji) => new()
    {
        Id = Guid.NewGuid(),
        SerialNumber = sn,
        Address = "Test 1",
        Latitude = 43.85m,
        Longitude = 18.41m,
        Type = MailboxType.WallSmall,
        Priority = p,
        Status = MailboxStatus.Prazan,
        Capacity = 100,
        InstallationYear = 2024
    };

    [Fact]
    public async Task GetPagedAsync_ClampsPageBelow1ToOne()
    {
        _repo.Setup(r => r.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>(), null, null, null, false, It.IsAny<CancellationToken>()))
             .ReturnsAsync((new List<Mailbox>(), 0));

        var result = await _sut.GetPagedAsync(0, 25, null, null, null, false, CancellationToken.None);

        Assert.Equal(1, result.Page);
        _repo.Verify(r => r.GetPagedAsync(1, 25, null, null, null, false, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetPagedAsync_ClampsPageSizeBelow1To25()
    {
        _repo.Setup(r => r.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>(), null, null, null, false, It.IsAny<CancellationToken>()))
             .ReturnsAsync((new List<Mailbox>(), 0));

        var result = await _sut.GetPagedAsync(1, 0, null, null, null, false, CancellationToken.None);

        Assert.Equal(25, result.PageSize);
        _repo.Verify(r => r.GetPagedAsync(1, 25, null, null, null, false, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetPagedAsync_ClampsPageSizeAbove100To100()
    {
        _repo.Setup(r => r.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>(), null, null, null, false, It.IsAny<CancellationToken>()))
             .ReturnsAsync((new List<Mailbox>(), 0));

        var result = await _sut.GetPagedAsync(1, 500, null, null, null, false, CancellationToken.None);

        Assert.Equal(100, result.PageSize);
    }

    [Fact]
    public async Task GetPagedAsync_PassesAllFiltersToRepository()
    {
        _repo.Setup(r => r.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<MailboxType?>(),
                                          It.IsAny<MailboxPriority?>(), It.IsAny<string?>(), It.IsAny<bool>(),
                                          It.IsAny<CancellationToken>()))
             .ReturnsAsync((new List<Mailbox>(), 0));

        await _sut.GetPagedAsync(2, 10, MailboxType.SpecialPriority, MailboxPriority.Visok, "Sarajevo", true, CancellationToken.None);

        _repo.Verify(r => r.GetPagedAsync(2, 10, MailboxType.SpecialPriority, MailboxPriority.Visok, "Sarajevo", true, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetPagedAsync_ReturnsItemsAndTotalsFromRepository()
    {
        var items = new List<Mailbox> { MakeMailbox("A"), MakeMailbox("B") };
        _repo.Setup(r => r.GetPagedAsync(1, 25, null, null, null, false, It.IsAny<CancellationToken>()))
             .ReturnsAsync((items, 42));

        var result = await _sut.GetPagedAsync(1, 25, null, null, null, false, CancellationToken.None);

        Assert.Equal(2, result.Items.Count);
        Assert.Equal(42, result.TotalCount);
        Assert.Equal(1, result.Page);
        Assert.Equal(25, result.PageSize);
        Assert.Equal(2, result.TotalPages);
    }

    [Fact]
    public async Task CreateAsync_SetsPriorityFromCommand()
    {
        _repo.Setup(r => r.SerialNumberExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        Mailbox? captured = null;
        _repo.Setup(r => r.AddAsync(It.IsAny<Mailbox>(), It.IsAny<CancellationToken>()))
             .Callback<Mailbox, CancellationToken>((m, _) => captured = m)
             .ReturnsAsync((Mailbox m, CancellationToken _) => m);

        var command = new CreateMailboxCommand(
            "SN1", "Adresa", 43.85m, 18.41m,
            MailboxType.SpecialPriority, 100, 2024,
            Notes: null, Priority: MailboxPriority.Visok);

        await _sut.CreateAsync(command, CancellationToken.None);

        Assert.NotNull(captured);
        Assert.Equal(MailboxPriority.Visok, captured!.Priority);
    }

    [Fact]
    public async Task CreateAsync_DefaultsPriorityToSrednjiWhenNotSpecified()
    {
        _repo.Setup(r => r.SerialNumberExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        Mailbox? captured = null;
        _repo.Setup(r => r.AddAsync(It.IsAny<Mailbox>(), It.IsAny<CancellationToken>()))
             .Callback<Mailbox, CancellationToken>((m, _) => captured = m)
             .ReturnsAsync((Mailbox m, CancellationToken _) => m);

        var command = new CreateMailboxCommand(
            "SN1", "Adresa", 43.85m, 18.41m,
            MailboxType.WallSmall, 100, 2024);

        await _sut.CreateAsync(command, CancellationToken.None);

        Assert.Equal(MailboxPriority.Srednji, captured!.Priority);
    }

    [Fact]
    public async Task CreateAsync_DefaultsStatusToPrazan()
    {
        _repo.Setup(r => r.SerialNumberExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        Mailbox? captured = null;
        _repo.Setup(r => r.AddAsync(It.IsAny<Mailbox>(), It.IsAny<CancellationToken>()))
             .Callback<Mailbox, CancellationToken>((m, _) => captured = m)
             .ReturnsAsync((Mailbox m, CancellationToken _) => m);

        var command = new CreateMailboxCommand(
            "SN1", "Adresa", 43.85m, 18.41m,
            MailboxType.WallSmall, 100, 2024);

        await _sut.CreateAsync(command, CancellationToken.None);

        Assert.Equal(MailboxStatus.Prazan, captured!.Status);
    }
}
