using System.ComponentModel.DataAnnotations;

namespace PostRoute.Api.Contracts.Users;

public record CreateUserRequest(
    [property: Required, MaxLength(50)] string FirstName,
    [property: Required, MaxLength(50)] string LastName,
    [property: Required, MinLength(3), MaxLength(30)] string Username,
    [property: Required, EmailAddress, MaxLength(100)] string Email,
    [property: Required, MinLength(8), MaxLength(128),
     RegularExpression(@"^(?=.*[A-Z])(?=.*\d).+$",
         ErrorMessage = "Lozinka mora sadržavati najmanje jedno veliko slovo i jedan broj.")]
    string Password
);
