namespace PostRoute.BLL.Exceptions;

public sealed class AccountLockedException : Exception
{
    public AccountLockedException() : base("Account is locked.") { }
}
