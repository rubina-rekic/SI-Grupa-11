using System.ComponentModel.DataAnnotations;
using PostRoute.DAL.Entities;

namespace PostRoute.Api.Contracts.Mailboxes;

public class UpdateMailboxRequest
{
    [Required(ErrorMessage = "Serijski broj je obavezan")]
    [MaxLength(50)]
    public string SerialNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Adresa je obavezna")]
    [MaxLength(200)]
    public string Address { get; set; } = string.Empty;

    [Required]
    [Range(-90, 90)]
    public decimal Latitude { get; set; }

    [Required]
    [Range(-180, 180)]
    public decimal Longitude { get; set; }

    [Required]
    public MailboxType Type { get; set; }

    public MailboxPriority Priority { get; set; } = MailboxPriority.Srednji;

    [Required]
    [Range(1, int.MaxValue)]
    public int Capacity { get; set; }

    [Required]
    [Range(1900, 2100)]
    public int InstallationYear { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    [MaxLength(200)]
    public string? Reason { get; set; }

    // US-32: Dostupnost
    public bool IsAlwaysAvailable { get; set; } = false;
    public TimeOnly? Slot1Start { get; set; }
    public TimeOnly? Slot1End { get; set; }
    public TimeOnly? Slot2Start { get; set; }
    public TimeOnly? Slot2End { get; set; }

    // US-33: Radni dani
    public MailboxWorkingDays WorkingDays { get; set; } = MailboxWorkingDays.RadniDani;
}