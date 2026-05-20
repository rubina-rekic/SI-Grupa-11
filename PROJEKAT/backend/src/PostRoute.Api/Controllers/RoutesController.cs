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
