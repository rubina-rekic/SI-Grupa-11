namespace PostRoute.Api.Contracts.Users;

public sealed record ChangePasswordRequest(
    string Email,
    string NewPassword
);