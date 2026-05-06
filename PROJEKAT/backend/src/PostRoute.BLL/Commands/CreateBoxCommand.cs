namespace PostRoute.BLL.Commands;

public record CreateBoxCommand(
	string Address,
	decimal Latitude,
	decimal Longitude,
	string Type,
	string SerialNumber,
	int Capacity,
	int YearOfInstallation
);
