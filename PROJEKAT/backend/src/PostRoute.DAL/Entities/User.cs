namespace PostRoute.DAL.Entities;

public sealed record User(
    Guid Id,
    string Username,
    string Email,
    string Role);
