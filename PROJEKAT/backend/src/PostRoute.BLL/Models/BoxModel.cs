namespace PostRoute.BLL.Models;

public sealed record BoxModel(
	Guid Id,
	string Address,
	decimal Latitude,
	decimal Longitude,
	string Type,
	string SerialNumber,
	int Capacity,
	int YearOfInstallation
);
