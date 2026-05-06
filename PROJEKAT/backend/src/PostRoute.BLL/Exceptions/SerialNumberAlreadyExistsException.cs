namespace PostRoute.BLL.Exceptions;

public sealed class SerialNumberAlreadyExistsException : Exception
{
	public SerialNumberAlreadyExistsException(string serialNumber)
		: base($"Sandučić sa serijskim brojem '{serialNumber}' je već registrovan.") { }
}
