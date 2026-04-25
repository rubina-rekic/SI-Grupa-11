namespace PostRoute.Api.Contracts.Users;

public record CreateUserRequest(
    string FirstName,
    string LastName,
    string Username,
    string Email,
    string Password
);
