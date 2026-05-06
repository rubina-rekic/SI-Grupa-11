using System.ComponentModel.DataAnnotations;

namespace PostRoute.Api.Contracts.Boxes;

public record CreateBoxRequest(
	[Required, MaxLength(255)] string Address,
	[Range(-90, 90)] decimal Latitude,
	[Range(-180, 180)] decimal Longitude,
	[Required] string Type,
	[Required, MaxLength(50)] string SerialNumber,
	[Range(1, int.MaxValue)] int Capacity,
	[Range(1900, 2100)] int YearOfInstallation
);
