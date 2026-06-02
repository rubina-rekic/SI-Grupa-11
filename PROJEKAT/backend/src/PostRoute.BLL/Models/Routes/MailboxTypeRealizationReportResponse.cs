namespace PostRoute.BLL.Models.Routes;

public class MailboxTypeRealizationReportResponse
{
    public DateOnly FromDate { get; set; }
    public DateOnly ToDate { get; set; }
    public int TotalTypes { get; set; }
    public int TotalPlannedEmpties { get; set; }
    public int TotalSuccessfulEmpties { get; set; }
    public int TotalProblemReports { get; set; }
    public decimal AverageFailureRate { get; set; }
    public List<MailboxTypeRealizationRowResponse> Rows { get; set; } = new();
}

public class MailboxTypeRealizationRowResponse
{
    public int TypeId { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public int PlannedEmpties { get; set; }
    public int SuccessfulEmpties { get; set; }
    public int ProblemReports { get; set; }
    public decimal FailureRate { get; set; }
    public List<MailboxTypeRealizationDetailResponse> Details { get; set; } = new();
}

public class MailboxTypeRealizationDetailResponse
{
    public Guid MailboxId { get; set; }
    public string Address { get; set; } = string.Empty;
    public DateOnly RouteDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
