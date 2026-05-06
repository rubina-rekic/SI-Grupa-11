namespace PostRoute.BLL.Exceptions;

public sealed class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException() : base("Invalid credentials.") { }
}
