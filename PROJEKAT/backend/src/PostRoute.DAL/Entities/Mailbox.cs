using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PostRoute.DAL.Entities;

public class Mailbox
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string SerialNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Address { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "decimal(9,6)")]
    [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
    public decimal Latitude { get; set; }

    [Required]
    [Column(TypeName = "decimal(9,6)")]
    [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
    public decimal Longitude { get; set; }

    [Required]
    public MailboxType Type { get; set; }

    [Required]
    public MailboxPriority Priority { get; set; } = MailboxPriority.Srednji;

    [Required]
    public MailboxStatus Status { get; set; } = MailboxStatus.Prazan;

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than 0")]
    public int Capacity { get; set; }

    [Required]
    [Range(1900, 2100, ErrorMessage = "Installation year must be reasonable")]
    public int InstallationYear { get; set; }

    // US-32: Radna dostupnost
    public bool IsAlwaysAvailable { get; set; } = false;

    [Column(TypeName = "time")]
    public TimeOnly? Slot1Start { get; set; }

    [Column(TypeName = "time")]
    public TimeOnly? Slot1End { get; set; }

    [Column(TypeName = "time")]
    public TimeOnly? Slot2Start { get; set; }

    [Column(TypeName = "time")]
    public TimeOnly? Slot2End { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(500)]
    public string? Notes { get; set; }

    public bool IsActive { get; set; } = true;
    
    public MailboxWorkingDays WorkingDays { get; set; } = MailboxWorkingDays.RadniDani;
}

[Flags]
public enum MailboxWorkingDays
{
    None = 0,
    Ponedjeljak = 1,
    Utorak = 2,
    Srijeda = 4,
    Cetvrtak = 8,
    Petak = 16,
    Subota = 32,
    Nedjelja = 64,
    RadniDani = Ponedjeljak | Utorak | Srijeda | Cetvrtak | Petak,
    Vikend = Subota | Nedjelja,
    SvakiDan = RadniDani | Vikend
}

public enum MailboxType
{
    [Display(Name = "Zidni (mali)")]
    WallSmall = 1,

    [Display(Name = "Samostojeći (veliki)")]
    StandaloneLarge = 2,

    [Display(Name = "Unutrašnji (stambene zgrade)")]
    IndoorResidential = 3,

    [Display(Name = "Specijalni (prioritetni)")]
    SpecialPriority = 4
}

public enum MailboxPriority
{
    [Display(Name = "Visok")]
    Visok = 1,

    [Display(Name = "Srednji")]
    Srednji = 2,

    [Display(Name = "Nizak")]
    Nizak = 3
}

public enum MailboxStatus
{
    [Display(Name = "Prazan")]
    Prazan = 0,

    [Display(Name = "Pun")]
    Pun = 1
}
