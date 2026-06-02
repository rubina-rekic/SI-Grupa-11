using Microsoft.AspNetCore.Mvc;
using Moq;
using PostRoute.Api.Controllers;
using PostRoute.BLL.Models.Routes;
using PostRoute.BLL.Services;
using Xunit;

namespace PostRoute.Api.Tests.Controllers;

public sealed class RoutesControllerTestsPBI037
{
    private readonly Mock<IRouteService> _routeServiceMock = new();
    private readonly RoutesController _sut;

    public RoutesControllerTestsPBI037()
    {
        _sut = new RoutesController(_routeServiceMock.Object);
    }

    [Fact]
    public async Task GetMailboxTypeRealizationReport_ShouldReturnOk_WithReport()
    {
        var fromDate = new DateOnly(2026, 5, 1);
        var toDate = new DateOnly(2026, 5, 31);
        var report = new MailboxTypeRealizationReportResponse
        {
            FromDate = fromDate,
            ToDate = toDate,
            TotalTypes = 2,
            TotalPlannedEmpties = 10,
            TotalSuccessfulEmpties = 7,
            TotalProblemReports = 3,
            AverageFailureRate = 30m,
            Rows = new List<MailboxTypeRealizationRowResponse>
            {
                new() { TypeId = 1, TypeName = "WallSmall", PlannedEmpties = 5, SuccessfulEmpties = 4, ProblemReports = 1, FailureRate = 20m }
            }
        };

        _routeServiceMock
            .Setup(s => s.GetMailboxTypeRealizationReportAsync(fromDate, toDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(report);

        var result = await _sut.GetMailboxTypeRealizationReport(fromDate, toDate);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<MailboxTypeRealizationReportResponse>(okResult.Value);
        Assert.Equal(2, response.TotalTypes);
        Assert.Equal(10, response.TotalPlannedEmpties);
        _routeServiceMock.Verify(
            s => s.GetMailboxTypeRealizationReportAsync(fromDate, toDate, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetMailboxTypeRealizationReport_ShouldReturnBadRequest_WhenPeriodMissing()
    {
        var result = await _sut.GetMailboxTypeRealizationReport(null, new DateOnly(2026, 5, 31));

        Assert.IsType<BadRequestObjectResult>(result);
        _routeServiceMock.Verify(
            s => s.GetMailboxTypeRealizationReportAsync(It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetMailboxTypeRealizationReport_ShouldReturnBadRequest_WhenServiceRejectsPeriod()
    {
        var fromDate = new DateOnly(2026, 5, 31);
        var toDate = new DateOnly(2026, 5, 1);

        _routeServiceMock
            .Setup(s => s.GetMailboxTypeRealizationReportAsync(fromDate, toDate, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Pocetni datum ne moze biti poslije zavrsnog datuma."));

        var result = await _sut.GetMailboxTypeRealizationReport(fromDate, toDate);

        Assert.IsType<BadRequestObjectResult>(result);
    }
}
