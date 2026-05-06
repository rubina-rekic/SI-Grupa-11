namespace PostRoute.DAL.Entities;

public sealed class Box
{
	public Guid Id { get; set; }
	public string Address { get; set; } = string.Empty;
	public decimal Latitude { get; set; }
	public decimal Longitude { get; set; }
	public string Type { get; set; } = string.Empty;
	public string SerialNumber { get; set; } = string.Empty;
	public int Capacity { get; set; }
	public int YearOfInstallation { get; set; }
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
