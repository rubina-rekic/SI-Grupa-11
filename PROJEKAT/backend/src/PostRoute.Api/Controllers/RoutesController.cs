using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostRoute.BLL.Models.Routes;
using PostRoute.BLL.Services;
using PostRoute.Domain.Entities;
using System.Security.Claims;

namespace PostRoute.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RoutesController : ControllerBase
{
    private readonly IRouteService _routeService;

    public RoutesController(IRouteService routeService)
    {
        _routeService = routeService;
    }

    [HttpGet]
    [Authorize(Roles = $"{UserRole.Administrator},{UserRole.Dispatcher}")]
    public async Task<IActionResult> GetByDate([FromQuery] DateOnly date)
    {
        var routes = await _routeService.GetRoutesForDateAsync(date);
        return Ok(routes);
    }

    [HttpGet("archive")]
    [Authorize(Roles = $"{UserRole.Administrator},{UserRole.Dispatcher}")]
    public async Task<IActionResult> GetArchive(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        [FromQuery] DateOnly? fromDate = null,
        [FromQuery] DateOnly? toDate = null,
        [FromQuery] Guid? postmanId = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _routeService.GetArchiveAsync(
            page, pageSize, fromDate, toDate, postmanId, cancellationToken);
        
        var response = new PostRoute.Api.Contracts.Common.PagedResponse<RouteResponse>(
            result.Items.ToList(),
            result.TotalCount,
            result.Page,
            result.PageSize,
            result.TotalPages
        );

        return Ok(response);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = $"{UserRole.Administrator},{UserRole.Dispatcher}")]
    public async Task<IActionResult> GetRouteDetails(Guid id)
    {
        var route = await _routeService.GetRouteDetailsAsync(id);
        if (route == null)
        {
            return NotFound(new { Message = "Ruta nije pronađena." });
        }

        return Ok(route);
    }

    [HttpGet("my-assigned-route/today")]
    [Authorize(Roles = UserRole.PostalWorker)]
    public async Task<IActionResult> GetMyAssignedRouteForToday()
    {
        var postmanId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(postmanId, out var postmanGuid))
        {
            return Unauthorized(new { Message = "Korisnik nije autentificiran." });
        }

        var route = await _routeService.GetPostmanAssignedRouteForTodayAsync(postmanGuid);
        if (route == null)
        {
            return Ok(new { Message = "Nema dodijeljene rute za danas." });
        }

        return Ok(route);
    }

    [HttpGet("{id}/available-postmen")]
    [Authorize(Roles = $"{UserRole.Administrator},{UserRole.Dispatcher}")]
    public async Task<IActionResult> GetAvailablePostmen(Guid id)
    {
        try
        {
            var postmen = await _routeService.GetAvailablePostmenAsync(id);
            return Ok(postmen);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }

    [HttpPut("{id}/assign")]
    [Authorize(Roles = $"{UserRole.Administrator},{UserRole.Dispatcher}")]
    public async Task<IActionResult> Assign(Guid id, [FromBody] AssignRouteRequest request)
    {
        var dispatcherName = User.FindFirst(ClaimTypes.Name)?.Value ?? "Nepoznat";

        try
        {
            var route = await _routeService.AssignRouteAsync(id, request, dispatcherName);
            return Ok(route);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPut("{id}/reorder")]
    [Authorize(Roles = $"{UserRole.Administrator},{UserRole.Dispatcher}")]
    public async Task<IActionResult> Reorder(Guid id, [FromBody] ReorderRouteRequest request)
    {
        var dispatcherName = User.FindFirst(ClaimTypes.Name)?.Value ?? "Nepoznat";

        try
        {
            var route = await _routeService.ReorderRouteAsync(id, request, dispatcherName);
            return Ok(route);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPost("generate")]
    [Authorize(Roles = $"{UserRole.Administrator},{UserRole.Dispatcher}")]
    public async Task<IActionResult> Generate([FromBody] GenerateRouteRequest request)
    {
        if (request.PostmanId == Guid.Empty)
        {
            return BadRequest(new { Message = "Postar je obavezan." });
        }

        var route = await _routeService.GenerateRouteAsync(request);

        if (route.RouteItems == null || route.RouteItems.Count == 0)
        {
            return BadRequest(new
            {
                Message = $"Nema dostupnih lokacija za generisanje rute. " +
                          $"Ukupno sanducica: {route.TotalMailboxesCount}, Aktivnih: {route.ActiveMailboxesCount}, " +
                          $"Obuhvacenih pravilima prioriteta: {route.DayFilteredMailboxesCount}."
            });
        }

        return Ok(route);
    }
}
