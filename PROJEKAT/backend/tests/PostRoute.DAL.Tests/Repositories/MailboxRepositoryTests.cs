using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using PostRoute.DAL.Entities;
using PostRoute.DAL.Repositories;
using Xunit;

namespace PostRoute.DAL.Tests.Repositories;

public sealed class MailboxRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly MailboxRepository _sut;

    public MailboxRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _sut = new MailboxRepository(_context);
    }

    [Fact]
    public async Task AddAsync_ShouldAddMailboxToDatabase()
    {
        // Arrange
        var mailbox = new Mailbox
        {
            Id = Guid.NewGuid(),
            SerialNumber = "SN001",
            Address = "Test Address",
            Latitude = 43.8563m,
            Longitude = 18.4131m,
            Type = MailboxType.WallSmall,
            Capacity = 100,
            InstallationYear = 2023,
            Notes = "Test notes"
        };

        // Act
        var result = await _sut.AddAsync(mailbox, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(mailbox.SerialNumber, result.SerialNumber);
        Assert.Equal(mailbox.Address, result.Address);
        Assert.True(result.CreatedAt > DateTime.MinValue);
        Assert.True(result.UpdatedAt > DateTime.MinValue);

        var savedMailbox = await _context.Mailboxes.FindAsync(mailbox.Id);
        Assert.NotNull(savedMailbox);
        Assert.Equal(mailbox.SerialNumber, savedMailbox.SerialNumber);
    }

    [Fact]
    public async Task GetByIdAsync_WhenMailboxExists_ShouldReturnMailbox()
    {
        // Arrange
        var mailboxId = Guid.NewGuid();
        var mailbox = new Mailbox
        {
            Id = mailboxId,
            SerialNumber = "SN001",
            Address = "Test Address",
            Latitude = 43.8563m,
            Longitude = 18.4131m,
            Type = MailboxType.WallSmall,
            Capacity = 100,
            InstallationYear = 2023
        };

        await _context.Mailboxes.AddAsync(mailbox);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetByIdAsync(mailboxId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(mailboxId, result.Id);
        Assert.Equal("SN001", result.SerialNumber);
    }

    [Fact]
    public async Task GetByIdAsync_WhenMailboxDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _sut.GetByIdAsync(nonExistentId, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetBySerialNumberAsync_WhenMailboxExists_ShouldReturnMailbox()
    {
        // Arrange
        var serialNumber = "SN001";
        var mailbox = new Mailbox
        {
            Id = Guid.NewGuid(),
            SerialNumber = serialNumber,
            Address = "Test Address",
            Latitude = 43.8563m,
            Longitude = 18.4131m,
            Type = MailboxType.WallSmall,
            Capacity = 100,
            InstallationYear = 2023
        };

        await _context.Mailboxes.AddAsync(mailbox);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetBySerialNumberAsync(serialNumber, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(serialNumber, result.SerialNumber);
    }

    [Fact]
    public async Task GetBySerialNumberAsync_WhenMailboxDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var nonExistentSerialNumber = "SN999";

        // Act
        var result = await _sut.GetBySerialNumberAsync(nonExistentSerialNumber, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllMailboxesOrderedBySerialNumber()
    {
        // Arrange
        var mailboxes = new[]
        {
            new Mailbox
            {
                Id = Guid.NewGuid(),
                SerialNumber = "SN003",
                Address = "Address 3",
                Latitude = 43.8563m,
                Longitude = 18.4131m,
                Type = MailboxType.WallSmall,
                Capacity = 100,
                InstallationYear = 2023
            },
            new Mailbox
            {
                Id = Guid.NewGuid(),
                SerialNumber = "SN001",
                Address = "Address 1",
                Latitude = 43.8563m,
                Longitude = 18.4131m,
                Type = MailboxType.StandaloneLarge,
                Capacity = 150,
                InstallationYear = 2022
            },
            new Mailbox
            {
                Id = Guid.NewGuid(),
                SerialNumber = "SN002",
                Address = "Address 2",
                Latitude = 43.8563m,
                Longitude = 18.4131m,
                Type = MailboxType.IndoorResidential,
                Capacity = 200,
                InstallationYear = 2024
            }
        };

        await _context.Mailboxes.AddRangeAsync(mailboxes);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetAllAsync(CancellationToken.None);

        // Assert
        Assert.Equal(3, result.Count());
        var resultList = result.ToList();
        Assert.Equal("SN001", resultList[0].SerialNumber);
        Assert.Equal("SN002", resultList[1].SerialNumber);
        Assert.Equal("SN003", resultList[2].SerialNumber);
    }

    [Fact]
    public async Task SerialNumberExistsAsync_WhenSerialNumberExists_ShouldReturnTrue()
    {
        // Arrange
        var serialNumber = "SN001";
        var mailbox = new Mailbox
        {
            Id = Guid.NewGuid(),
            SerialNumber = serialNumber,
            Address = "Test Address",
            Latitude = 43.8563m,
            Longitude = 18.4131m,
            Type = MailboxType.WallSmall,
            Capacity = 100,
            InstallationYear = 2023
        };

        await _context.Mailboxes.AddAsync(mailbox);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.SerialNumberExistsAsync(serialNumber, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task SerialNumberExistsAsync_WhenSerialNumberDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var nonExistentSerialNumber = "SN999";

        // Act
        var result = await _sut.SerialNumberExistsAsync(nonExistentSerialNumber, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateMailboxInDatabase()
    {
        // Arrange
        var mailboxId = Guid.NewGuid();
        var originalMailbox = new Mailbox
        {
            Id = mailboxId,
            SerialNumber = "SN001",
            Address = "Original Address",
            Latitude = 43.8563m,
            Longitude = 18.4131m,
            Type = MailboxType.WallSmall,
            Capacity = 100,
            InstallationYear = 2023,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        await _context.Mailboxes.AddAsync(originalMailbox);
        await _context.SaveChangesAsync();

        // Get the mailbox to update
        var mailboxToUpdate = await _context.Mailboxes.FindAsync(mailboxId);
        mailboxToUpdate!.Address = "Updated Address";
        mailboxToUpdate.Capacity = 150;

        // Act
        var result = await _sut.UpdateAsync(mailboxToUpdate, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Address", result.Address);
        Assert.Equal(150, result.Capacity);

        var updatedMailbox = await _context.Mailboxes.FindAsync(mailboxId);
        Assert.Equal("Updated Address", updatedMailbox!.Address);
        Assert.Equal(150, updatedMailbox.Capacity);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveMailboxFromDatabase()
    {
        // Arrange
        var mailboxId = Guid.NewGuid();
        var mailbox = new Mailbox
        {
            Id = mailboxId,
            SerialNumber = "SN001",
            Address = "Test Address",
            Latitude = 43.8563m,
            Longitude = 18.4131m,
            Type = MailboxType.WallSmall,
            Capacity = 100,
            InstallationYear = 2023
        };

        await _context.Mailboxes.AddAsync(mailbox);
        await _context.SaveChangesAsync();

        // Verify mailbox exists
        var existingMailbox = await _context.Mailboxes.FindAsync(mailboxId);
        Assert.NotNull(existingMailbox);

        // Act
        await _sut.DeleteAsync(mailboxId, CancellationToken.None);

        // Assert
        var deletedMailbox = await _context.Mailboxes.FindAsync(mailboxId);
        Assert.Null(deletedMailbox);
    }

    [Fact]
    public async Task DeleteAsync_WhenMailboxDoesNotExist_ShouldNotThrowException()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act & Assert - Should not throw
        await _sut.DeleteAsync(nonExistentId, CancellationToken.None);

        // Verify no exception was thrown and method completed
        Assert.True(true);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
