using Microsoft.AspNetCore.Mvc;
using Moq;
using PostRoute.Api.Contracts.Common;
using PostRoute.Api.Controllers;
using PostRoute.BLL.Models;
using PostRoute.BLL.Models.Routes;
using PostRoute.BLL.Services;
using Xunit;

namespace PostRoute.Api.Tests.Controllers;

public sealed class RoutesControllerTestsPBI034
{
    private readonly Mock<IRouteService> _routeServiceMock = new();
    private readonly RoutesController _sut;

    public RoutesControllerTestsPBI034()
    {
        _sut = new RoutesController(_routeServiceMock.Object);
    }

    [Fact]
    public async Task GetArchive_ShouldPassFiltersToService_AndReturnPagedResponse()
    {
        var postmanId = Guid.NewGuid();
        var fromDate = new DateOnly(2026, 5, 1);
        var toDate = new DateOnly(2026, 5, 30);
        var route = new RouteResponse
        {
            Id = Guid.NewGuid(),
            PostmanId = postmanId,
            PostmanName = "Postar Test",
            Date = toDate,
            Status = "Zavrsena"
        };

        _routeServiceMock
            .Setup(s => s.GetArchiveAsync(2, 10, fromDate, toDate, postmanId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<RouteResponse>(new List<RouteResponse> { route }, 11, 2, 10));

        var result = await _sut.GetArchive(2, 10, fromDate, toDate, postmanId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<PagedResponse<RouteResponse>>(okResult.Value);
        Assert.Single(response.Items);
        Assert.Equal(11, response.TotalCount);
        Assert.Equal(2, response.Page);
        _routeServiceMock.Verify(s => s.GetArchiveAsync(2, 10, fromDate, toDate, postmanId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetRouteDetails_ShouldReturnUnavailableReason_ForArchivedRouteDetails()
    {
        var routeId = Guid.NewGuid();
        var route = new RouteResponse
        {
            Id = routeId,
            PostmanId = Guid.NewGuid(),
            Date = new DateOnly(2026, 5, 30),
            Status = "Zavrsena",
            RouteItems = new List<RouteItemResponse>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    MailboxId = Guid.NewGuid(),
                    Address = "Titova 1",
                    Status = "Nedostupan",
                    MailboxStatus = "Nedostupan",
                    ProcessedStatus = "Nedostupan",
                    ProcessedAt = new DateTime(2026, 5, 30, 8, 20, 0, DateTimeKind.Utc),
                    UnavailableReason = "Zakljucan pristup"
                }
            }
        };

        _routeServiceMock
            .Setup(s => s.GetRouteDetailsAsync(routeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(route);

        var result = await _sut.GetRouteDetails(routeId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<RouteResponse>(okResult.Value);
        Assert.Equal("Zakljucan pristup", response.RouteItems[0].UnavailableReason);
        Assert.Equal("Nedostupan", response.RouteItems[0].ProcessedStatus);
    }
}
