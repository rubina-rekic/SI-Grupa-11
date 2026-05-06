using Microsoft.AspNetCore.Mvc;
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
                request.Notes
            );

            var mailbox = await _mailboxService.CreateAsync(command, cancellationToken);
            
            var response = new MailboxResponse(
                mailbox.Id,
                mailbox.SerialNumber,
                mailbox.Address,
                mailbox.Latitude,
                mailbox.Longitude,
                mailbox.Type,
                mailbox.Capacity,
                mailbox.InstallationYear,
                mailbox.CreatedAt,
                mailbox.UpdatedAt,
                mailbox.Notes
            );

            return Created($"/api/mailboxes/{mailbox.Id}", response);
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

        var response = new MailboxResponse(
            mailbox.Id,
            mailbox.SerialNumber,
            mailbox.Address,
            mailbox.Latitude,
            mailbox.Longitude,
            mailbox.Type,
            mailbox.Capacity,
            mailbox.InstallationYear,
            mailbox.CreatedAt,
            mailbox.UpdatedAt,
            mailbox.Notes
        );

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MailboxResponse>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var mailboxes = await _mailboxService.GetAllAsync(cancellationToken);
        
        var response = mailboxes.Select(m => new MailboxResponse(
            m.Id,
            m.SerialNumber,
            m.Address,
            m.Latitude,
            m.Longitude,
            m.Type,
            m.Capacity,
            m.InstallationYear,
            m.CreatedAt,
            m.UpdatedAt,
            m.Notes
        ));

        return Ok(response);
    }

    [HttpGet("check-serial-number/{serialNumber}")]
    public async Task<ActionResult<bool>> CheckSerialNumberExistsAsync(string serialNumber, CancellationToken cancellationToken)
    {
        var exists = await _mailboxService.SerialNumberExistsAsync(serialNumber, cancellationToken);
        return Ok(exists);
    }
}
