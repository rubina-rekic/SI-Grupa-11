namespace PostRoute.Domain.Entities;

public static class BoxType
{
	public const string WallSmall = "Zidni (mali)";
	public const string StandaloneSmall = "Samostojeći (veliki)";
	public const string IndoorResidential = "Unutrašnji (stambene zgrade)";
	public const string SpecialPriority = "Specijalni (prioritetni)";

	public static readonly string[] AllTypes =
	[
		WallSmall,
		StandaloneSmall,
		IndoorResidential,
		SpecialPriority,
	];

	public static bool IsValidType(string? type) =>
		type != null && AllTypes.Contains(type);
}
