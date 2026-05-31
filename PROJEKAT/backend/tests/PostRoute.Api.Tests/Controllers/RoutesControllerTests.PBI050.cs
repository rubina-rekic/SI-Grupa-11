using Microsoft.AspNetCore.Mvc;
using Moq;
using PostRoute.Api.Controllers;
using PostRoute.BLL.Models.Routes;
using PostRoute.BLL.Services;
using Xunit;

namespace PostRoute.Api.Tests.Controllers;

public sealed class RoutesControllerTestsPBI050
{
    private readonly Mock<IRouteService> _routeServiceMock = new();
    private readonly RoutesController _sut;

    public RoutesControllerTestsPBI050()
    {
        _sut = new RoutesController(_routeServiceMock.Object);
    }

    [Fact]
    public async Task GetPostmanPerformanceReport_ShouldReturnOk_WithReport()
    {
        var fromDate = new DateOnly(2026, 5, 1);
        var toDate = new DateOnly(2026, 5, 31);
        var report = new PostmanPerformanceReportResponse
        {
            FromDate = fromDate,
            ToDate = toDate,
            TotalPostmen = 1,
            Rows = new List<PostmanPerformanceRowResponse>
            {
                new()
                {
                    PostmanId = Guid.NewGuid(),
                    PostmanName = "Ibrahim Test",
                    AssignedMailboxes = 3,
                    EmptiedLocations = 2,
                    UnrealizedLocations = 1,
                    SuccessPercentage = 66.67m,
                    CompletedRoutesCount = 1
                }
            }
        };

        _routeServiceMock
            .Setup(s => s.GetPostmanPerformanceReportAsync(fromDate, toDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(report);

        var result = await _sut.GetPostmanPerformanceReport(fromDate, toDate);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<PostmanPerformanceReportResponse>(okResult.Value);
        Assert.Equal(1, response.TotalPostmen);
        Assert.Equal("Ibrahim Test", response.Rows[0].PostmanName);
        _routeServiceMock.Verify(
            s => s.GetPostmanPerformanceReportAsync(fromDate, toDate, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetPostmanPerformanceReport_ShouldReturnBadRequest_WhenPeriodMissing()
    {
        var result = await _sut.GetPostmanPerformanceReport(null, new DateOnly(2026, 5, 31));

        Assert.IsType<BadRequestObjectResult>(result);
        _routeServiceMock.Verify(
            s => s.GetPostmanPerformanceReportAsync(It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetPostmanPerformanceReport_ShouldReturnBadRequest_WhenServiceRejectsPeriod()
    {
        var fromDate = new DateOnly(2026, 5, 31);
        var toDate = new DateOnly(2026, 5, 1);

        _routeServiceMock
            .Setup(s => s.GetPostmanPerformanceReportAsync(fromDate, toDate, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Pocetni datum ne moze biti poslije zavrsnog datuma."));

        var result = await _sut.GetPostmanPerformanceReport(fromDate, toDate);

        Assert.IsType<BadRequestObjectResult>(result);
    }
}
