namespace PostRoute.Api.Contracts.Users;

public sealed record UserResponse(
    Guid Id,
    string Username,
    string Email,
    string Role);
