using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostRoute.BLL.Models.Routes;
using PostRoute.BLL.Services;
using PostRoute.Domain.Entities;

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

    [HttpPost("generate")]
    [Authorize(Roles = $"{UserRole.Administrator},{UserRole.Dispatcher}")]
    public async Task<IActionResult> Generate([FromBody] GenerateRouteRequest request)
    {
        if (request.PostmanId == Guid.Empty)
        {
            return BadRequest(new { Message = "Poštar je obavezan." });
        }

        var route = await _routeService.GenerateRouteAsync(request);
        
        // Prijavljeno je da rute često ostaju prazne zbog filtriranja pa vraćamo sve podatke u oba slučaja radi transparentnosti.
        if (route.RouteItems == null || route.RouteItems.Count == 0)
        {
            return BadRequest(new { 
                Message = $"Nema dostupnih lokacija za generisanje rute. " +
                          $"Ukupno sandučića: {route.TotalMailboxesCount}, Aktivnih: {route.ActiveMailboxesCount}, Obuhvaćenih radnim danom i opcijama dostupnosti: {route.DayFilteredMailboxesCount}." 
            });
        }
        
        return Ok(route);
    }
}
