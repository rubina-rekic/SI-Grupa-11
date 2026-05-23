using Microsoft.AspNetCore.Mvc;
using PostRoute.Api.Contracts.Common;
using PostRoute.Api.Contracts.Mailboxes;
using PostRoute.Api.Middleware;
using PostRoute.BLL.Commands;
using PostRoute.BLL.Services;
using PostRoute.DAL.Entities;
namespace PostRoute.Api.Controllers;

// Ovaj kontroler je namijenjen samo za administratore, ali GET endpointi su javni
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
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var command = new CreateMailboxCommand(
    request.SerialNumber, request.Address, request.Latitude, request.Longitude,
    request.Type, request.Capacity, request.InstallationYear, request.Notes,
    request.Priority, request.Reason,
    request.IsAlwaysAvailable,
    request.Slot1Start, request.Slot1End, request.Slot2Start, request.Slot2End,
    request.WorkingDays
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

    [HttpPut("{id:guid}")]
    [RequiredRole("Administrator")]
    public async Task<ActionResult<MailboxResponse>> UpdateAsync(
        Guid id,
        [FromBody] UpdateMailboxRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                return Unauthorized("User ID not found or invalid");
            }

            var command = new UpdateMailboxCommand(
     id, request.SerialNumber, request.Address, request.Latitude, request.Longitude,
     request.Type, request.Priority, request.Capacity, request.InstallationYear,
     request.Notes, userGuid, request.Reason,
     request.IsAlwaysAvailable,
     request.Slot1Start, request.Slot1End, request.Slot2Start, request.Slot2End,
     request.WorkingDays
 );

            var mailbox = await _mailboxService.UpdateAsync(command, cancellationToken);

            return Ok(MapToResponse(mailbox));
        }
        catch (InvalidOperationException ex)
        {
            if (ex.Message.Contains("nije pronađen"))
            {
                return NotFound();
            }
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    [RequiredRole("Administrator")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _mailboxService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("nije pronadjen") || ex.Message.Contains("nije prona"))
        {
            return NotFound();
        }
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

    [HttpGet("{id:guid}/history")]
    [RequiredRole("Administrator")]
    public async Task<ActionResult> GetHistoryAsync(
    Guid id,
    CancellationToken cancellationToken)
    {
        var logs = await _mailboxService.GetAuditLogAsync(id, cancellationToken);

        var response = logs.Select(log => new
        {
            log.Id,
            log.MailboxId,
            log.UserId,
            Username = log.User.Username,
            log.FieldName,
            log.OldValue,
            log.NewValue,
            log.Action,
            log.Reason,
            log.Timestamp
        });

        return Ok(response);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<MailboxResponse>> UpdateStatusAsync(
        Guid id,
        [FromBody] UpdateMailboxStatusRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            return Unauthorized(new { message = "Korisnik nije autentificiran." });

        try
        {
            var command = new UpdateMailboxStatusCommand(id, request.Status, userGuid, request.Reason);
            var mailbox = await _mailboxService.UpdateStatusAsync(command, cancellationToken);
            return Ok(MapToResponse(mailbox));
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("nije pronađen"))
        {
            return NotFound();
        }
    }

    private static MailboxResponse MapToResponse(Mailbox m) => new(
     m.Id, m.SerialNumber, m.Address, m.Latitude, m.Longitude,
     m.Type, m.Priority, m.Status, m.Capacity, m.InstallationYear,
     m.CreatedAt, m.UpdatedAt, m.Notes,
     m.IsAlwaysAvailable, m.Slot1Start, m.Slot1End, m.Slot2Start, m.Slot2End,
     m.WorkingDays
 );
}
