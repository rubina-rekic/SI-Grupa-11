using Moq;
using PostRoute.BLL.Commands;
using PostRoute.BLL.Services;
using PostRoute.DAL.Entities;
using PostRoute.DAL.Repositories;
using Xunit;

namespace PostRoute.BLL.Tests.Services;

/// <summary>
/// Unit testovi za PBI-021 (US-32): Definisanje vremenskih okvira dostupnosti sandučića
/// 
/// Testira sljedeće scenarije:
/// - Kreiranje sandučića sa 24/7 dostupnosti
/// - Kreiranje sandučića sa jednim vremenskim okvirom
/// - Kreiranje sandučića sa dva vremenska okvira (sa pauzom)
/// - Validacija da kraj vremena mora biti nakon početka
/// - Validacija da se vremeni okviri ne preklapaju
/// - Validacija da je barem jedan vremenski okvir potreban ako nije 24/7
/// </summary>
public sealed class MailboxServiceTestsPBI021
{
    private readonly Mock<IMailboxRepository> _mailboxRepositoryMock;
    private readonly Mock<IMailboxAuditLogRepository> _auditLogRepositoryMock;
    private readonly MailboxService _sut;

    public MailboxServiceTestsPBI021()
    {
        _mailboxRepositoryMock = new Mock<IMailboxRepository>();
        _auditLogRepositoryMock = new Mock<IMailboxAuditLogRepository>();
        _sut = new MailboxService(_mailboxRepositoryMock.Object, _auditLogRepositoryMock.Object);
    }

    // ============================================
    // SCENARIO 1: 24/7 Dostupnost (IsAlwaysAvailable = true)
    // ============================================

    [Fact]
    public async Task CreateAsync_WithAlwaysAvailable_ShouldCreateSuccessfully()
    {
        // Arrange - Sandučić koji je dostupan 24/7
        var command = new CreateMailboxCommand(
            "24H-001",
            "24/7 Available Mailbox",
            43.8563m,
            18.4131m,
            MailboxType.WallSmall,
            100,
            2024,
            Notes: "Open around the clock",
            Priority: MailboxPriority.Srednji,
            Reason: null,
            IsAlwaysAvailable: true,
            Slot1Start: null,
            Slot1End: null,
            Slot2Start: null,
            Slot2End: null,
            WorkingDays: MailboxWorkingDays.Ponedjeljak | MailboxWorkingDays.Utorak | 
                         MailboxWorkingDays.Srijeda | MailboxWorkingDays.Cetvrtak | 
                         MailboxWorkingDays.Petak | MailboxWorkingDays.Subota | 
                         MailboxWorkingDays.Nedjelja
        );

        var expectedMailbox = new Mailbox
        {
            Id = Guid.NewGuid(),
            SerialNumber = command.SerialNumber,
            Address = command.Address,
            IsAlwaysAvailable = true,
            Slot1Start = null,
            Slot1End = null,
            Slot2Start = null,
            Slot2End = null
        };

        _mailboxRepositoryMock
            .Setup(x => x.SerialNumberExistsAsync(command.SerialNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mailboxRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Mailbox>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedMailbox);

        // Act
        var result = await _sut.CreateAsync(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsAlwaysAvailable);
        Assert.Null(result.Slot1Start);
        Assert.Null(result.Slot1End);
    }

    // ============================================
    // SCENARIO 2: Jedan vremenski okvir (08:00-17:00)
    // ============================================

    [Fact]
    public async Task CreateAsync_WithSingleTimeSlot_ShouldCreateSuccessfully()
    {
        // Arrange - Sandučić dostupan samo u radnom vremenu
        var command = new CreateMailboxCommand(
            "WORK-001",
            "Office Hours Only",
            43.8563m,
            18.4131m,
            MailboxType.WallSmall,
            100,
            2024,
            Notes: "Available during working hours",
            Priority: MailboxPriority.Srednji,
            Reason: null,
            IsAlwaysAvailable: false,
            Slot1Start: new TimeOnly(8, 0),
            Slot1End: new TimeOnly(17, 0),
            Slot2Start: null,
            Slot2End: null,
            WorkingDays: MailboxWorkingDays.Ponedjeljak | MailboxWorkingDays.Utorak | 
                         MailboxWorkingDays.Srijeda | MailboxWorkingDays.Cetvrtak | 
                         MailboxWorkingDays.Petak
        );

        var expectedMailbox = new Mailbox
        {
            Id = Guid.NewGuid(),
            SerialNumber = command.SerialNumber,
            Address = command.Address,
            IsAlwaysAvailable = false,
            Slot1Start = new TimeOnly(8, 0),
            Slot1End = new TimeOnly(17, 0),
            Slot2Start = null,
            Slot2End = null
        };

        _mailboxRepositoryMock
            .Setup(x => x.SerialNumberExistsAsync(command.SerialNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mailboxRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Mailbox>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedMailbox);

        // Act
        var result = await _sut.CreateAsync(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsAlwaysAvailable);
        Assert.Equal(new TimeOnly(8, 0), result.Slot1Start);
        Assert.Equal(new TimeOnly(17, 0), result.Slot1End);
        Assert.Null(result.Slot2Start);
        Assert.Null(result.Slot2End);
    }

    // ============================================
    // SCENARIO 3: Dva vremenska okvira sa pauzom
    // (08:00-12:00) + (14:00-18:00)
    // ============================================

    [Fact]
    public async Task CreateAsync_WithTwoTimeSlots_ShouldCreateSuccessfully()
    {
        // Arrange - Sandučić sa pauzom u podne
        var command = new CreateMailboxCommand(
            "SPLIT-001",
            "Split Schedule - Morning and Afternoon",
            43.8563m,
            18.4131m,
            MailboxType.IndoorResidential,
            150,
            2023,
            Notes: "Closed during lunch break (12:00-14:00)",
            Priority: MailboxPriority.Visok,
            Reason: null,
            IsAlwaysAvailable: false,
            Slot1Start: new TimeOnly(8, 0),
            Slot1End: new TimeOnly(12, 0),
            Slot2Start: new TimeOnly(14, 0),
            Slot2End: new TimeOnly(18, 0),
            WorkingDays: MailboxWorkingDays.Ponedjeljak | MailboxWorkingDays.Utorak | 
                         MailboxWorkingDays.Srijeda | MailboxWorkingDays.Cetvrtak | 
                         MailboxWorkingDays.Petak
        );

        var expectedMailbox = new Mailbox
        {
            Id = Guid.NewGuid(),
            SerialNumber = command.SerialNumber,
            Address = command.Address,
            IsAlwaysAvailable = false,
            Slot1Start = new TimeOnly(8, 0),
            Slot1End = new TimeOnly(12, 0),
            Slot2Start = new TimeOnly(14, 0),
            Slot2End = new TimeOnly(18, 0)
        };

        _mailboxRepositoryMock
            .Setup(x => x.SerialNumberExistsAsync(command.SerialNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mailboxRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Mailbox>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedMailbox);

        // Act
        var result = await _sut.CreateAsync(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsAlwaysAvailable);
        Assert.Equal(new TimeOnly(8, 0), result.Slot1Start);
        Assert.Equal(new TimeOnly(12, 0), result.Slot1End);
        Assert.Equal(new TimeOnly(14, 0), result.Slot2Start);
        Assert.Equal(new TimeOnly(18, 0), result.Slot2End);
    }

    // ============================================
    // SCENARIO 4: Greška - Kraj prije početka
    // ============================================

    [Fact]
    public async Task CreateAsync_WithEndTimeBeforeStartTime_ShouldThrow()
    {
        // Arrange - Nevaljani vremenski okvir (kraj prije početka)
        var command = new CreateMailboxCommand(
            "INVALID-001",
            "Invalid Times",
            43.8563m,
            18.4131m,
            MailboxType.WallSmall,
            100,
            2024,
            Notes: null,
            Priority: MailboxPriority.Srednji,
            Reason: null,
            IsAlwaysAvailable: false,
            Slot1Start: new TimeOnly(17, 0),
            Slot1End: new TimeOnly(8, 0),  // Kraj prije početka!
            Slot2Start: null,
            Slot2End: null,
            WorkingDays: MailboxWorkingDays.Ponedjeljak
        );

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.CreateAsync(command, CancellationToken.None));
        
        Assert.Contains("Krajnje vrijeme mora biti nakon početnog", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    // ============================================
    // SCENARIO 5: Greška - Vremeni okviri se preklapaju
    // ============================================

    [Fact]
    public async Task CreateAsync_WithOverlappingTimeSlots_ShouldThrow()
    {
        // Arrange - Drugi termin se preklapa sa prvim
        var command = new CreateMailboxCommand(
            "OVERLAP-001",
            "Overlapping Times",
            43.8563m,
            18.4131m,
            MailboxType.StandaloneLarge,
            200,
            2024,
            Notes: null,
            Priority: MailboxPriority.Srednji,
            Reason: null,
            IsAlwaysAvailable: false,
            Slot1Start: new TimeOnly(8, 0),
            Slot1End: new TimeOnly(12, 0),
            Slot2Start: new TimeOnly(11, 30),  // Počinje prije nego što je prvi završen!
            Slot2End: new TimeOnly(18, 0),
            WorkingDays: MailboxWorkingDays.Ponedjeljak
        );

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.CreateAsync(command, CancellationToken.None));
        
        Assert.Contains("preklapa", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    // ============================================
    // SCENARIO 6: Greška - Nema vremenske okvira ako nije 24/7
    // ============================================

    [Fact]
    public async Task CreateAsync_WithoutTimeSlotAndNotAlwaysAvailable_ShouldThrow()
    {
        // Arrange - Nije 24/7 a nema ni jednog vremenskog okvira
        var command = new CreateMailboxCommand(
            "EMPTY-001",
            "No Time Slots",
            43.8563m,
            18.4131m,
            MailboxType.WallSmall,
            100,
            2024,
            Notes: null,
            Priority: MailboxPriority.Srednji,
            Reason: null,
            IsAlwaysAvailable: false,
            Slot1Start: null,
            Slot1End: null,
            Slot2Start: null,
            Slot2End: null,
            WorkingDays: MailboxWorkingDays.Ponedjeljak
        );

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.CreateAsync(command, CancellationToken.None));
        
        Assert.Contains("barem jedan vremenski period", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    // ============================================
    // SCENARIO 7: Ažuriranje sa novim vremenima
    // ============================================

    [Fact]
    public async Task UpdateAsync_WithNewTimeSlots_ShouldUpdateSuccessfully()
    {
        // Arrange
        var id = Guid.NewGuid();
        var existingMailbox = new Mailbox
        {
            Id = id,
            SerialNumber = "UPDATE-001",
            Address = "Update Test",
            Latitude = 43.85m,
            Longitude = 18.41m,
            Type = MailboxType.WallSmall,
            Priority = MailboxPriority.Srednji,
            Status = MailboxStatus.Prazan,
            Capacity = 100,
            InstallationYear = 2024,
            IsAlwaysAvailable = true
        };

        var updateCommand = new UpdateMailboxCommand(
            Id: id,
            SerialNumber: "UPDATE-001",
            Address: "Update Test",
            Latitude: 43.85m,
            Longitude: 18.41m,
            Type: MailboxType.WallSmall,
            Priority: MailboxPriority.Srednji,
            Capacity: 100,
            InstallationYear: 2024,
            Notes: "Updated",
            UserId: Guid.NewGuid(),
            Reason: "Update availability",
            IsAlwaysAvailable: false,
            Slot1Start: new TimeOnly(9, 0),
            Slot1End: new TimeOnly(17, 0),
            Slot2Start: null,
            Slot2End: null,
            WorkingDays: MailboxWorkingDays.Ponedjeljak
        );

        _mailboxRepositoryMock
            .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingMailbox);

        _mailboxRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Mailbox>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingMailbox);

        // Act
        var result = await _sut.UpdateAsync(updateCommand, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        _mailboxRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<Mailbox>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    // ============================================
    // SCENARIO 8: Edge case - Midnight hours
    // ============================================

    [Fact]
    public async Task CreateAsync_WithMidnightHours_ShouldCreateSuccessfully()
    {
        // Arrange - Noćni rad (00:00-06:00)
        var command = new CreateMailboxCommand(
            "NIGHT-001",
            "Night Shift Mailbox",
            43.8563m,
            18.4131m,
            MailboxType.WallSmall,
            100,
            2024,
            Notes: "Available during night shift",
            Priority: MailboxPriority.Srednji,
            Reason: null,
            IsAlwaysAvailable: false,
            Slot1Start: new TimeOnly(0, 0),
            Slot1End: new TimeOnly(6, 0),
            Slot2Start: null,
            Slot2End: null,
            WorkingDays: MailboxWorkingDays.Ponedjeljak
        );

        var expectedMailbox = new Mailbox
        {
            Id = Guid.NewGuid(),
            SerialNumber = command.SerialNumber,
            Address = command.Address,
            IsAlwaysAvailable = false,
            Slot1Start = new TimeOnly(0, 0),
            Slot1End = new TimeOnly(6, 0)
        };

        _mailboxRepositoryMock
            .Setup(x => x.SerialNumberExistsAsync(command.SerialNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mailboxRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Mailbox>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedMailbox);

        // Act
        var result = await _sut.CreateAsync(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(new TimeOnly(0, 0), result.Slot1Start);
        Assert.Equal(new TimeOnly(6, 0), result.Slot1End);
    }
}
