namespace PostRoute.BLL.Models;

public sealed record UserModel(
    Guid Id,
    string Username,
    string Email,
    string Role);
