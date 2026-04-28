namespace PostRoute.DAL.Entities;

public sealed class SecurityLog
{
    public Guid Id { get; set; }
    public string? UserId { get; set; }
    public string AttemptedUrl { get; set; } = string.Empty;
    public string? UserRole { get; set; }
    public string? IpAddress { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string AccessType { get; set; } = string.Empty; // "Unauthorized", "Forbidden"
    public string? UserAgent { get; set; }
    public bool IsSuccessful { get; set; }
}
