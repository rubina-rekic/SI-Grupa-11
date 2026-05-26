using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostRoute.BLL.Services;
using PostRoute.DAL.Entities;
using PostRoute.Domain.Entities;
using System.Security.Claims;

namespace PostRoute.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class IssuesController : ControllerBase
{
    private readonly IIssueService _issueService;

    public IssuesController(IIssueService issueService)
    {
        _issueService = issueService;
    }

    [HttpGet]
    [Authorize(Roles = $"{UserRole.Administrator},{UserRole.Dispatcher}")]
    public async Task<IActionResult> GetAll(
        [FromQuery] IssueStatus? status,
        CancellationToken cancellationToken)
    {
        var issues = await _issueService.GetAllAsync(status, cancellationToken);
        return Ok(issues);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = $"{UserRole.Administrator},{UserRole.Dispatcher}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var issue = await _issueService.GetByIdAsync(id, cancellationToken);
            return Ok(issue);
        }
        catch (InvalidOperationException)
        {
            return NotFound(new { Message = "Problem nije pronađen." });
        }
    }

    [HttpPost("{id:guid}/comments")]
    public async Task<IActionResult> AddComment(
        Guid id,
        [FromBody] AddCommentRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
            return BadRequest(new { Message = "Komentar ne može biti prazan." });

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

        if (!Guid.TryParse(userId, out var userGuid))
            return Unauthorized(new { Message = "Korisnik nije autentificiran." });

        try
        {
            var issue = await _issueService.AddCommentAsync(id, userGuid, userRole ?? "", request.Content, cancellationToken);
            return Ok(issue);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPut("{id:guid}/action")]
    [Authorize(Roles = $"{UserRole.Administrator},{UserRole.Dispatcher}")]
    public async Task<IActionResult> AssignAction(
        Guid id,
        [FromBody] AssignActionRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userId, out var userGuid))
            return Unauthorized(new { Message = "Korisnik nije autentificiran." });

        try
        {
            var issue = await _issueService.AssignActionAsync(id, userGuid, request.Action, request.TargetPostmanId, cancellationToken);
            return Ok(issue);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPut("{id:guid}/resolve")]
    [Authorize(Roles = $"{UserRole.Administrator},{UserRole.Dispatcher}")]
    public async Task<IActionResult> Resolve(Guid id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userId, out var userGuid))
            return Unauthorized(new { Message = "Korisnik nije autentificiran." });

        try
        {
            var issue = await _issueService.ResolveAsync(id, userGuid, cancellationToken);
            return Ok(issue);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpGet("worker/{id:guid}")]
[Authorize(Roles = UserRole.PostalWorker)]
public async Task<IActionResult> GetByIdForWorker(Guid id, CancellationToken cancellationToken)
{
    try
    {
        var issue = await _issueService.GetByIdAsync(id, cancellationToken);
        return Ok(issue);
    }
    catch (InvalidOperationException)
    {
        return NotFound(new { Message = "Problem nije pronađen." });
    }
}

    [HttpGet("my-notifications")]
    [Authorize(Roles = UserRole.PostalWorker)]
    public async Task<IActionResult> GetMyNotifications(CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userId, out var userGuid))
            return Unauthorized(new { Message = "Korisnik nije autentificiran." });

        var notifications = await _issueService.GetMyNotificationsAsync(userGuid, cancellationToken);
        return Ok(notifications);
    }

    [HttpPut("notifications/{notificationId:guid}/read")]
    [Authorize(Roles = UserRole.PostalWorker)]
    public async Task<IActionResult> MarkNotificationRead(Guid notificationId, CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userId, out var userGuid))
            return Unauthorized(new { Message = "Korisnik nije autentificiran." });

        try
        {
            await _issueService.MarkNotificationReadAsync(notificationId, userGuid, cancellationToken);
            return Ok(new { Message = "Notifikacija označena kao pročitana." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }
}

public record AddCommentRequest(string Content);
public record AssignActionRequest(IssueAction Action, Guid? TargetPostmanId);