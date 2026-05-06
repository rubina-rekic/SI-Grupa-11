namespace PostRoute.Api.Contracts.Boxes;

public sealed record BoxResponse(
	Guid Id,
	string Address,
	decimal Latitude,
	decimal Longitude,
	string Type,
	string SerialNumber,
	int Capacity,
	int YearOfInstallation
);
