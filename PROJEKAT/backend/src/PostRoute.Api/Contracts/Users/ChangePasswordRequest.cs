using System.ComponentModel.DataAnnotations;

namespace PostRoute.Api.Contracts.Users;

public sealed record ChangePasswordRequest(
    [Required, EmailAddress] string Email,
    [Required] string CurrentPassword,
    [Required, MinLength(8), RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,}$",
        ErrorMessage = "Lozinka mora imati najmanje 8 karaktera, veliko slovo, broj i specijalni znak.")]
    string NewPassword
);