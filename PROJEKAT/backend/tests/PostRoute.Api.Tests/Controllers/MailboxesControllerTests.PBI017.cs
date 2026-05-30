using Microsoft.AspNetCore.Mvc;
using Moq;
using PostRoute.Api.Contracts.Common;
using PostRoute.Api.Contracts.Mailboxes;
using PostRoute.Api.Controllers;
using PostRoute.BLL.Commands;
using PostRoute.BLL.Models;
using PostRoute.BLL.Services;
using PostRoute.DAL.Entities;
using Xunit;

namespace PostRoute.Api.Tests.Controllers;

public sealed class MailboxesControllerTestsPBI017
{
    private readonly Mock<IMailboxService> _mailboxServiceMock = new();
    private readonly MailboxesController _sut;

    public MailboxesControllerTestsPBI017()
    {
        _sut = new MailboxesController(_mailboxServiceMock.Object);
    }

    private static Mailbox MakeMailbox(string serialNumber = "TEST001") => new()
    {
        Id = Guid.NewGuid(),
        SerialNumber = serialNumber,
        Address = "Test Address 1",
        Latitude = 43.8563m,
        Longitude = 18.4131m,
        Type = MailboxType.WallSmall,
        Priority = MailboxPriority.Srednji,
        Status = MailboxStatus.Prazan,
        Capacity = 100,
        InstallationYear = 2024,
        Notes = "Test notes"
    };

    [Fact]
    public async Task CreateAsync_ShouldReturnCreatedResult_WhenValidRequest()
    {
        var request = new CreateMailboxRequest
        {
            SerialNumber = "TEST001",
            Address = "Test Address 1",
            Latitude = 43.8563m,
            Longitude = 18.4131m,
            Type = MailboxType.WallSmall,
            Priority = MailboxPriority.Srednji,
            Capacity = 100,
            InstallationYear = 2024,
            Notes = "Test notes"
        };
        var expectedMailbox = MakeMailbox(request.SerialNumber);

        _mailboxServiceMock
            .Setup(x => x.CreateAsync(It.IsAny<CreateMailboxCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedMailbox);

        var result = await _sut.CreateAsync(request, CancellationToken.None);

        var createdResult = Assert.IsType<CreatedResult>(result.Result);
        Assert.Equal($"/api/mailboxes/{expectedMailbox.Id}", createdResult.Location);
        var response = Assert.IsType<MailboxResponse>(createdResult.Value);
        Assert.Equal(expectedMailbox.Id, response.Id);
        Assert.Equal(expectedMailbox.SerialNumber, response.SerialNumber);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnBadRequest_WhenModelStateInvalid()
    {
        _sut.ModelState.AddModelError("SerialNumber", "Serijski broj je obavezan");

        var result = await _sut.CreateAsync(new CreateMailboxRequest(), CancellationToken.None);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnConflict_WhenSerialNumberExists()
    {
        var request = new CreateMailboxRequest
        {
            SerialNumber = "EXIST001",
            Address = "Test Address",
            Latitude = 43.8563m,
            Longitude = 18.4131m,
            Type = MailboxType.WallSmall,
            Capacity = 100,
            InstallationYear = 2024
        };

        _mailboxServiceMock
            .Setup(x => x.CreateAsync(It.IsAny<CreateMailboxCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Sanducic sa ovim serijskim brojem vec postoji"));

        var result = await _sut.CreateAsync(request, CancellationToken.None);

        var conflictResult = Assert.IsType<ConflictObjectResult>(result.Result);
        Assert.Equal(409, conflictResult.StatusCode);
    }

    [Fact]
    public async Task CheckSerialNumberExistsAsync_ShouldReturnBoolean()
    {
        _mailboxServiceMock
            .Setup(x => x.SerialNumberExistsAsync("EXIST001", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.CheckSerialNumberExistsAsync("EXIST001", CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.True(Assert.IsType<bool>(okResult.Value));
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnPagedMailboxes()
    {
        var mailboxes = new List<Mailbox>
        {
            MakeMailbox("TEST001"),
            MakeMailbox("TEST002")
        };

        _mailboxServiceMock
            .Setup(x => x.GetPagedAsync(1, 25, null, null, null, null, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<Mailbox>(mailboxes, mailboxes.Count, 1, 25));

        var result = await _sut.GetAllAsync();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<PagedResponse<MailboxResponse>>(okResult.Value);
        Assert.Equal(2, response.Items.Count);
        Assert.Equal(2, response.TotalCount);
    }

    [Fact]
    public async Task GetAllAsync_ShouldPassTypePriorityStatusAndSearchFilters()
    {
        _mailboxServiceMock
            .Setup(x => x.GetPagedAsync(
                1,
                25,
                MailboxType.WallSmall,
                MailboxPriority.Visok,
                MailboxStatus.Pun,
                "SN001",
                false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<Mailbox>(new List<Mailbox>(), 0, 1, 25));

        await _sut.GetAllAsync(
            type: MailboxType.WallSmall,
            priority: MailboxPriority.Visok,
            status: MailboxStatus.Pun,
            search: "SN001");

        _mailboxServiceMock.Verify(x => x.GetPagedAsync(
            1,
            25,
            MailboxType.WallSmall,
            MailboxPriority.Visok,
            MailboxStatus.Pun,
            "SN001",
            false,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnMailbox_WhenMailboxExists()
    {
        var mailbox = MakeMailbox();

        _mailboxServiceMock
            .Setup(x => x.GetByIdAsync(mailbox.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mailbox);

        var result = await _sut.GetByIdAsync(mailbox.Id, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<MailboxResponse>(okResult.Value);
        Assert.Equal(mailbox.Id, response.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNotFound_WhenMailboxDoesNotExist()
    {
        var mailboxId = Guid.NewGuid();

        _mailboxServiceMock
            .Setup(x => x.GetByIdAsync(mailboxId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Mailbox?)null);

        var result = await _sut.GetByIdAsync(mailboxId, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result.Result);
    }
}
