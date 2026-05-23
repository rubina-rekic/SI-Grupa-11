using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PostRoute.Api.Controllers;
using PostRoute.BLL.Models.Routes;
using PostRoute.BLL.Services;
using Xunit;

namespace PostRoute.Api.Tests.Controllers;

public sealed class RoutesControllerTestsPBI029
{
    private readonly Mock<IRouteService> _routeServiceMock = new();
    private readonly RoutesController _sut;

    public RoutesControllerTestsPBI029()
    {
        _sut = new RoutesController(_routeServiceMock.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(
                        new[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                            new Claim(ClaimTypes.Name, "dispatcher@postroute.ba"),
                            new Claim("role", "Dispatcher")
                        },
                        "TestAuth"))
                }
            }
        };
    }

    private static RouteResponse MakeRouteResponse(string status = "Dodijeljena") => new()
    {
        Id = Guid.NewGuid(),
        PostmanId = Guid.NewGuid(),
        PostmanName = "Amar Hodžić",
        Date = new DateOnly(2026, 5, 23),
        PlannedStartTime = new TimeOnly(8, 0),
        PlannedEndTime = new TimeOnly(10, 0),
        TotalDistanceKm = 12.5m,
        TotalDurationMinutes = 120,
        Status = status,
        ExceedsStandardTime = false,
        RouteItems = new List<RouteItemResponse>
        {
            new()
            {
                Id = Guid.NewGuid(),
                MailboxId = Guid.NewGuid(),
                Address = "Titova 1, Sarajevo",
                Latitude = 43.85m,
                Longitude = 18.41m,
                Order = 1,
                EstimatedArrivalTime = TimeOnly.Parse("08:15:00"),
                Priority = "Visok",
                Status = "Planirano",
                IsManuallyReordered = false,
                MailboxStatus = "Prazan"
            }
        }
    };

    // ================================================================
    // HAPPY PATH
    // ================================================================

    [Fact]
    public async Task GetByDate_ShouldReturnOk_WithListOfRoutes()
    {
        var date = new DateOnly(2026, 5, 23);
        var routes = new List<RouteResponse>
        {
            MakeRouteResponse("UProgresu"),
            MakeRouteResponse("Dodijeljena"),
        };

        _routeServiceMock
            .Setup(s => s.GetRoutesForDateAsync(date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(routes);

        var result = await _sut.GetByDate(date) as OkObjectResult;

        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        var returned = Assert.IsType<List<RouteResponse>>(result.Value);
        Assert.Equal(2, returned.Count);
    }

    [Fact]
    public async Task GetByDate_ShouldReturnOk_WithEmptyList_WhenNoRoutesExist()
    {
        var date = new DateOnly(2026, 5, 23);

        _routeServiceMock
            .Setup(s => s.GetRoutesForDateAsync(date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<RouteResponse>());

        var result = await _sut.GetByDate(date) as OkObjectResult;

        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        var returned = Assert.IsType<List<RouteResponse>>(result.Value);
        Assert.Empty(returned);
    }

    [Fact]
    public async Task GetByDate_ShouldCallService_WithCorrectDate()
    {
        var date = new DateOnly(2026, 5, 23);

        _routeServiceMock
            .Setup(s => s.GetRoutesForDateAsync(It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<RouteResponse>());

        await _sut.GetByDate(date);

        _routeServiceMock.Verify(
            s => s.GetRoutesForDateAsync(date, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetByDate_ShouldReturnAllStatuses_InSingleCall()
    {
        var date = new DateOnly(2026, 5, 23);
        var routes = new List<RouteResponse>
        {
            MakeRouteResponse("UProgresu"),
            MakeRouteResponse("Dodijeljena"),
            MakeRouteResponse("Planirana"),
            MakeRouteResponse("Zavrsena"),
            MakeRouteResponse("Otkazana"),
        };

        _routeServiceMock
            .Setup(s => s.GetRoutesForDateAsync(date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(routes);

        var result = await _sut.GetByDate(date) as OkObjectResult;

        Assert.NotNull(result);
        var returned = Assert.IsType<List<RouteResponse>>(result.Value);
        Assert.Equal(5, returned.Count);
        Assert.Contains(returned, r => r.Status == "UProgresu");
        Assert.Contains(returned, r => r.Status == "Zavrsena");
    }

    [Fact]
    public async Task GetByDate_ShouldReturnRouteItems_WithMailboxStatus()
    {
        var date = new DateOnly(2026, 5, 23);
        var route = MakeRouteResponse("UProgresu");
        route.RouteItems[0].MailboxStatus = "Napunjen";

        _routeServiceMock
            .Setup(s => s.GetRoutesForDateAsync(date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<RouteResponse> { route });

        var result = await _sut.GetByDate(date) as OkObjectResult;

        Assert.NotNull(result);
        var returned = Assert.IsType<List<RouteResponse>>(result.Value);
        Assert.Equal("Napunjen", returned[0].RouteItems[0].MailboxStatus);
    }
}
