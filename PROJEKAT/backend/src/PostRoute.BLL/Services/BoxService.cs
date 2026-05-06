using PostRoute.BLL.Commands;
using PostRoute.BLL.Exceptions;
using PostRoute.BLL.Models;
using PostRoute.DAL.Entities;
using PostRoute.DAL.Repositories;
using PostRoute.Domain.Entities;

namespace PostRoute.BLL.Services;

public sealed class BoxService : IBoxService
{
	private readonly IBoxRepository _repository;

	public BoxService(IBoxRepository repository)
	{
		_repository = repository;
	}

	public async Task<BoxModel?> GetByIdAsync(Guid boxId, CancellationToken ct)
	{
		var box = await _repository.GetByIdAsync(boxId, ct);
		return box == null ? null : MapToModel(box);
	}

	public async Task<BoxModel> CreateAsync(CreateBoxCommand command, CancellationToken ct)
	{
		if (!BoxType.IsValidType(command.Type))
		{
			throw new ArgumentException("Nevalidan tip sandučića.", nameof(command.Type));
		}

		if (command.Latitude < -90 || command.Latitude > 90)
		{
			throw new ArgumentException("Latitude mora biti između -90 i 90.", nameof(command.Latitude));
		}

		if (command.Longitude < -180 || command.Longitude > 180)
		{
			throw new ArgumentException("Longitude mora biti između -180 i 180.", nameof(command.Longitude));
		}

		if (await _repository.SerialNumberExistsAsync(command.SerialNumber, ct))
		{
			throw new SerialNumberAlreadyExistsException(command.SerialNumber);
		}

		var box = new Box
		{
			Id = Guid.NewGuid(),
			Address = command.Address,
			Latitude = command.Latitude,
			Longitude = command.Longitude,
			Type = command.Type,
			SerialNumber = command.SerialNumber,
			Capacity = command.Capacity,
			YearOfInstallation = command.YearOfInstallation,
		};

		await _repository.AddAsync(box, ct);

		return MapToModel(box);
	}

	public async Task<IEnumerable<BoxModel>> GetAllAsync(CancellationToken ct)
	{
		var boxes = await _repository.GetAllAsync(ct);
		return boxes.Select(MapToModel).ToList();
	}

	private static BoxModel MapToModel(Box box) =>
		new(
			box.Id,
			box.Address,
			box.Latitude,
			box.Longitude,
			box.Type,
			box.SerialNumber,
			box.Capacity,
			box.YearOfInstallation
		);
}
