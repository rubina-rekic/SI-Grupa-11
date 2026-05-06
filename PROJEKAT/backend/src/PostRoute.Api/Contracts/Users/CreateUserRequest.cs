using System.ComponentModel.DataAnnotations;

namespace PostRoute.Api.Contracts.Users;

public record CreateUserRequest(
    [Required, MaxLength(50)] string FirstName,
    [Required, MaxLength(50)] string LastName,
    [Required, MinLength(3), MaxLength(30)] string Username,
    [Required, EmailAddress, MaxLength(100)] string Email,
    [Required, MinLength(8), MaxLength(128),
     RegularExpression(@"^(?=.*[A-Z])(?=.*\d).+$",
         ErrorMessage = "Lozinka mora sadržavati najmanje jedno veliko slovo i jedan broj.")]
    string Password,
    string? Role
);
