using Microsoft.AspNetCore.Mvc;
using PostRoute.Api.Contracts.Boxes;
using PostRoute.Api.Middleware;
using PostRoute.BLL.Commands;
using PostRoute.BLL.Exceptions;
using PostRoute.BLL.Services;

namespace PostRoute.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class BoxesController : ControllerBase
{
	private readonly IBoxService _boxService;

	public BoxesController(IBoxService boxService)
	{
		_boxService = boxService;
	}

	[HttpGet("{boxId:guid}")]
	public async Task<ActionResult<BoxResponse>> GetByIdAsync(Guid boxId, CancellationToken cancellationToken)
	{
		var box = await _boxService.GetByIdAsync(boxId, cancellationToken);

		if (box is null)
		{
			return NotFound();
		}

		var response = new BoxResponse(
			box.Id,
			box.Address,
			box.Latitude,
			box.Longitude,
			box.Type,
			box.SerialNumber,
			box.Capacity,
			box.YearOfInstallation
		);
		return Ok(response);
	}

	[HttpPost]
	[RequiredRole("Administrator")]
	public async Task<ActionResult<BoxResponse>> CreateAsync(
		[FromBody] CreateBoxRequest request,
		CancellationToken cancellationToken)
	{
		try
		{
			var command = new CreateBoxCommand(
				request.Address,
				request.Latitude,
				request.Longitude,
				request.Type,
				request.SerialNumber,
				request.Capacity,
				request.YearOfInstallation
			);

			var box = await _boxService.CreateAsync(command, cancellationToken);
			var response = new BoxResponse(
				box.Id,
				box.Address,
				box.Latitude,
				box.Longitude,
				box.Type,
				box.SerialNumber,
				box.Capacity,
				box.YearOfInstallation
			);

			return Created($"/api/boxes/{box.Id}", response);
		}
		catch (SerialNumberAlreadyExistsException ex)
		{
			return Conflict(new { message = ex.Message });
		}
		catch (ArgumentException ex)
		{
			return BadRequest(new { message = ex.Message });
		}
	}

	[HttpGet]
	[RequiredRole("Administrator")]
	public async Task<ActionResult<IEnumerable<BoxResponse>>> GetAllAsync(CancellationToken cancellationToken)
	{
		var boxes = await _boxService.GetAllAsync(cancellationToken);
		var response = boxes.Select(b => new BoxResponse(
			b.Id,
			b.Address,
			b.Latitude,
			b.Longitude,
			b.Type,
			b.SerialNumber,
			b.Capacity,
			b.YearOfInstallation
		));
		return Ok(response);
	}
}
