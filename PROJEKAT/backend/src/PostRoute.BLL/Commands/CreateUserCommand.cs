namespace PostRoute.BLL.Commands;

public record CreateUserCommand(
    string FirstName,
    string LastName,
    string Username,
    string Email,
    string Password,
    string Role
);
