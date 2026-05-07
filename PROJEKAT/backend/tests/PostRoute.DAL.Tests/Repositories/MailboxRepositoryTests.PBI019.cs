using Microsoft.EntityFrameworkCore;
using PostRoute.DAL.Entities;
using PostRoute.DAL.Repositories;
using Xunit;

namespace PostRoute.DAL.Tests.Repositories;

public sealed class MailboxRepositoryTestsPBI019 : IDisposable
{
    private readonly AppDbContext _context;
    private readonly MailboxRepository _sut;

    public MailboxRepositoryTestsPBI019()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        _sut = new MailboxRepository(_context);
    }

    private async Task SeedAsync()
    {
        _context.Mailboxes.AddRange(
            new Mailbox { Id = Guid.NewGuid(), SerialNumber = "SN001", Address = "Sarajevo, Titova 1", Latitude = 43.85m, Longitude = 18.41m, Type = MailboxType.WallSmall, Priority = MailboxPriority.Visok, Status = MailboxStatus.Prazan, Capacity = 100, InstallationYear = 2020 },
            new Mailbox { Id = Guid.NewGuid(), SerialNumber = "SN002", Address = "Sarajevo, Maršala Tita 5", Latitude = 43.85m, Longitude = 18.41m, Type = MailboxType.StandaloneLarge, Priority = MailboxPriority.Srednji, Status = MailboxStatus.Pun, Capacity = 200, InstallationYear = 2021 },
            new Mailbox { Id = Guid.NewGuid(), SerialNumber = "SN003", Address = "Mostar, Glavna 10", Latitude = 43.34m, Longitude = 17.81m, Type = MailboxType.WallSmall, Priority = MailboxPriority.Nizak, Status = MailboxStatus.Prazan, Capacity = 80, InstallationYear = 2022 },
            new Mailbox { Id = Guid.NewGuid(), SerialNumber = "SN004", Address = "Tuzla, Centar 2", Latitude = 44.54m, Longitude = 18.66m, Type = MailboxType.SpecialPriority, Priority = MailboxPriority.Visok, Status = MailboxStatus.Pun, Capacity = 150, InstallationYear = 2023 }
        );
        await _context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetPagedAsync_ReturnsAllWithoutFilters()
    {
        await SeedAsync();
        var (items, total) = await _sut.GetPagedAsync(1, 25, null, null, null, false, CancellationToken.None);
        Assert.Equal(4, total);
        Assert.Equal(4, items.Count);
    }

    [Fact]
    public async Task GetPagedAsync_FiltersByType()
    {
        await SeedAsync();
        var (items, total) = await _sut.GetPagedAsync(1, 25, MailboxType.WallSmall, null, null, false, CancellationToken.None);
        Assert.Equal(2, total);
        Assert.All(items, m => Assert.Equal(MailboxType.WallSmall, m.Type));
    }

    [Fact]
    public async Task GetPagedAsync_FiltersByPriority()
    {
        await SeedAsync();
        var (items, total) = await _sut.GetPagedAsync(1, 25, null, MailboxPriority.Visok, null, false, CancellationToken.None);
        Assert.Equal(2, total);
        Assert.All(items, m => Assert.Equal(MailboxPriority.Visok, m.Priority));
    }

    [Fact]
    public async Task GetPagedAsync_FiltersByAddressCaseInsensitive()
    {
        await SeedAsync();
        var (items, total) = await _sut.GetPagedAsync(1, 25, null, null, "sarajevo", false, CancellationToken.None);
        Assert.Equal(2, total);
        Assert.All(items, m => Assert.Contains("Sarajevo", m.Address, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task GetPagedAsync_SortsByPriorityWhenFlagSet()
    {
        await SeedAsync();
        var (items, _) = await _sut.GetPagedAsync(1, 25, null, null, null, true, CancellationToken.None);
        Assert.Equal(MailboxPriority.Visok, items[0].Priority);
        Assert.Equal(MailboxPriority.Visok, items[1].Priority);
        Assert.Equal(MailboxPriority.Srednji, items[2].Priority);
        Assert.Equal(MailboxPriority.Nizak, items[3].Priority);
    }

    [Fact]
    public async Task GetPagedAsync_DefaultSortBySerialNumberWhenFlagOff()
    {
        await SeedAsync();
        var (items, _) = await _sut.GetPagedAsync(1, 25, null, null, null, false, CancellationToken.None);
        Assert.Equal("SN001", items[0].SerialNumber);
        Assert.Equal("SN002", items[1].SerialNumber);
        Assert.Equal("SN003", items[2].SerialNumber);
        Assert.Equal("SN004", items[3].SerialNumber);
    }

    [Fact]
    public async Task GetPagedAsync_PaginatesCorrectly()
    {
        await SeedAsync();
        var (page1, total1) = await _sut.GetPagedAsync(1, 2, null, null, null, false, CancellationToken.None);
        var (page2, total2) = await _sut.GetPagedAsync(2, 2, null, null, null, false, CancellationToken.None);

        Assert.Equal(4, total1);
        Assert.Equal(4, total2);
        Assert.Equal(2, page1.Count);
        Assert.Equal(2, page2.Count);
        Assert.Equal("SN001", page1[0].SerialNumber);
        Assert.Equal("SN003", page2[0].SerialNumber);
    }

    [Fact]
    public async Task GetPagedAsync_CombinesFilters()
    {
        await SeedAsync();
        var (items, total) = await _sut.GetPagedAsync(1, 25, MailboxType.WallSmall, MailboxPriority.Visok, null, false, CancellationToken.None);
        Assert.Equal(1, total);
        Assert.Equal("SN001", items[0].SerialNumber);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
