using Microsoft.AspNetCore.Mvc;
using PostRoute.Api.Contracts.Common;
using PostRoute.Api.Contracts.Mailboxes;
using PostRoute.Api.Middleware;
using PostRoute.BLL.Commands;
using PostRoute.BLL.Services;
using PostRoute.DAL.Entities;

namespace PostRoute.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class MailboxesController : ControllerBase
{
    private readonly IMailboxService _mailboxService;

    public MailboxesController(IMailboxService mailboxService)
    {
        _mailboxService = mailboxService;
    }

    [HttpPost]
    [RequiredRole("Administrator")]
    public async Task<ActionResult<MailboxResponse>> CreateAsync(
        [FromBody] CreateMailboxRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new CreateMailboxCommand(
                request.SerialNumber,
                request.Address,
                request.Latitude,
                request.Longitude,
                request.Type,
                request.Capacity,
                request.InstallationYear,
                request.Notes,
                request.Priority
            );

            var mailbox = await _mailboxService.CreateAsync(command, cancellationToken);

            return Created($"/api/mailboxes/{mailbox.Id}", MapToResponse(mailbox));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MailboxResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var mailbox = await _mailboxService.GetByIdAsync(id, cancellationToken);

        if (mailbox is null)
        {
            return NotFound();
        }

        return Ok(MapToResponse(mailbox));
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<MailboxResponse>>> GetAllAsync(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        [FromQuery] MailboxType? type = null,
        [FromQuery] MailboxPriority? priority = null,
        [FromQuery] string? search = null,
        [FromQuery] bool sortByPriority = false,
        CancellationToken cancellationToken = default)
    {
        var result = await _mailboxService.GetPagedAsync(
            page, pageSize, type, priority, search, sortByPriority, cancellationToken);

        var response = new PagedResponse<MailboxResponse>(
            result.Items.Select(MapToResponse).ToList(),
            result.TotalCount,
            result.Page,
            result.PageSize,
            result.TotalPages
        );

        return Ok(response);
    }

    [HttpGet("check-serial-number/{serialNumber}")]
    public async Task<ActionResult<bool>> CheckSerialNumberExistsAsync(string serialNumber, CancellationToken cancellationToken)
    {
        var exists = await _mailboxService.SerialNumberExistsAsync(serialNumber, cancellationToken);
        return Ok(exists);
    }

    private static MailboxResponse MapToResponse(Mailbox m) => new(
        m.Id,
        m.SerialNumber,
        m.Address,
        m.Latitude,
        m.Longitude,
        m.Type,
        m.Priority,
        m.Status,
        m.Capacity,
        m.InstallationYear,
        m.CreatedAt,
        m.UpdatedAt,
        m.Notes
    );
}
