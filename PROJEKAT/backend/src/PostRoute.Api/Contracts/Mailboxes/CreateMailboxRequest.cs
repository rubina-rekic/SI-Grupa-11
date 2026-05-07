using System.ComponentModel.DataAnnotations;
using PostRoute.DAL.Entities;

namespace PostRoute.Api.Contracts.Mailboxes;

public class CreateMailboxRequest
{
    [Required(ErrorMessage = "Serijski broj je obavezan")]
    [MaxLength(50, ErrorMessage = "Serijski broj ne može biti duži od 50 karaktera")]
    public string SerialNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Adresa je obavezna")]
    [MaxLength(200, ErrorMessage = "Adresa ne može biti duža od 200 karaktera")]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "Latitude je obavezan")]
    [Range(-90, 90, ErrorMessage = "Latitude mora biti između -90 i 90")]
    public decimal Latitude { get; set; }

    [Required(ErrorMessage = "Longitude je obavezan")]
    [Range(-180, 180, ErrorMessage = "Longitude mora biti između -180 i 180")]
    public decimal Longitude { get; set; }

    [Required(ErrorMessage = "Tip sandučića je obavezan")]
    public MailboxType Type { get; set; }

    public MailboxPriority Priority { get; set; } = MailboxPriority.Srednji;

    [Required(ErrorMessage = "Kapacitet je obavezan")]
    [Range(1, int.MaxValue, ErrorMessage = "Kapacitet mora biti veći od 0")]
    public int Capacity { get; set; }

    [Required(ErrorMessage = "Godina instalacije je obavezna")]
    [Range(1900, 2100, ErrorMessage = "Godina instalacije mora biti razumljiva")]
    public int InstallationYear { get; set; }

    [MaxLength(500, ErrorMessage = "Napomene ne mogu biti duže od 500 karaktera")]
    public string? Notes { get; set; }
}
