namespace PostRoute.BLL.Models.Routes;

public class AvailablePostmanResponse
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public bool IsCurrentAssignee { get; set; }
    public string? UnavailableReason { get; set; }
}
