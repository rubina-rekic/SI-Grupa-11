using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PostRoute.Api.Controllers;
using PostRoute.BLL.Models.Routes;
using PostRoute.BLL.Services;
using Xunit;

namespace PostRoute.Api.Tests.Controllers;

public sealed class RoutesControllerTestsPBI026
{
    private readonly Mock<IRouteService> _routeServiceMock = new();
    private readonly RoutesController _sut;

    public RoutesControllerTestsPBI026()
    {
        var postmanId = Guid.NewGuid();
        _sut = new RoutesController(_routeServiceMock.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(
                        new[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, postmanId.ToString()),
                            new Claim(ClaimTypes.Name, "postar@mail.com"),
                            new Claim("role", "PostalWorker")
                        },
                        "TestAuth"))
                }
            }
        };
    }

    private static RouteResponse MakeRouteResponse(Guid? postmanId = null) => new()
    {
        Id = Guid.NewGuid(),
        PostmanId = postmanId ?? Guid.NewGuid(),
        PostmanName = "Poštar Test",
        Date = DateOnly.FromDateTime(DateTime.UtcNow),
        PlannedStartTime = new TimeOnly(8, 0),
        PlannedEndTime = new TimeOnly(10, 0),
        TotalDistanceKm = 12.5m,
        TotalDurationMinutes = 120,
        Status = "Dodijeljena",
        ExceedsStandardTime = false,
        AssignedAt = DateTime.UtcNow,
        AssignedBy = "dispatcher",
        RouteItems = new List<RouteItemResponse>
        {
            new()
            {
                Id = Guid.NewGuid(),
                MailboxId = Guid.NewGuid(),
                Address = "Sarajevo, Test 1",
                Latitude = 43.85m,
                Longitude = 18.41m,
                Order = 1,
                EstimatedArrivalTime = TimeOnly.Parse("08:15:00"),
                Priority = "Visok",
                Status = "Čeka",
                IsManuallyReordered = false
            },
            new()
            {
                Id = Guid.NewGuid(),
                MailboxId = Guid.NewGuid(),
                Address = "Sarajevo, Test 2",
                Latitude = 43.86m,
                Longitude = 18.42m,
                Order = 2,
                EstimatedArrivalTime = TimeOnly.Parse("08:30:00"),
                Priority = "Srednji",
                Status = "Čeka",
                IsManuallyReordered = false
            }
        }
    };

    [Fact]
    public async Task GetMyAssignedRouteForToday_ShouldReturnOk_WithAssignedRoute()
    {
        // Arrange
        var postmanId = Guid.Parse(_sut.ControllerContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var route = MakeRouteResponse(postmanId);

        _routeServiceMock
            .Setup(service => service.GetPostmanAssignedRouteForTodayAsync(postmanId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(route);

        // Act
        var result = await _sut.GetMyAssignedRouteForToday() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.IsType<RouteResponse>(result.Value);
        
        var returnedRoute = (RouteResponse)result.Value!;
        Assert.Equal(route.Id, returnedRoute.Id);
        Assert.Equal(postmanId, returnedRoute.PostmanId);
        Assert.Equal("Dodijeljena", returnedRoute.Status);
        Assert.Equal(2, returnedRoute.RouteItems.Count);
    }

    [Fact]
    public async Task GetMyAssignedRouteForToday_ShouldReturnUnauthorized_WhenUserNotAuthenticated()
    {
        // Arrange
        var controllerWithoutAuth = new RoutesController(_routeServiceMock.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity())
                }
            }
        };

        // Act
        var result = await controllerWithoutAuth.GetMyAssignedRouteForToday() as UnauthorizedObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(401, result.StatusCode);
    }

    [Fact]
    public async Task GetMyAssignedRouteForToday_ShouldReturnOk_WithInProgressRoute()
    {
        // Arrange
        var postmanId = Guid.Parse(_sut.ControllerContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var route = MakeRouteResponse(postmanId);
        route.Status = "U toku";

        _routeServiceMock
            .Setup(service => service.GetPostmanAssignedRouteForTodayAsync(postmanId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(route);

        // Act
        var result = await _sut.GetMyAssignedRouteForToday() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        
        var returnedRoute = (RouteResponse)result.Value!;
        Assert.Equal("U toku", returnedRoute.Status);
    }

    [Fact]
    public async Task GetMyAssignedRouteForToday_ShouldCallServiceWithCorrectPostmanId()
    {
        // Arrange
        var postmanId = Guid.Parse(_sut.ControllerContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var route = MakeRouteResponse(postmanId);

        _routeServiceMock
            .Setup(service => service.GetPostmanAssignedRouteForTodayAsync(postmanId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(route);

        // Act
        await _sut.GetMyAssignedRouteForToday();

        // Assert
        _routeServiceMock.Verify(
            service => service.GetPostmanAssignedRouteForTodayAsync(postmanId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetMyAssignedRouteForToday_ShouldIncludeRouteItems_WithCompleteData()
    {
        // Arrange
        var postmanId = Guid.Parse(_sut.ControllerContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var route = MakeRouteResponse(postmanId);

        _routeServiceMock
            .Setup(service => service.GetPostmanAssignedRouteForTodayAsync(postmanId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(route);

        // Act
        var result = await _sut.GetMyAssignedRouteForToday() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var returnedRoute = (RouteResponse)result.Value!;

        Assert.NotEmpty(returnedRoute.RouteItems);
        var firstItem = returnedRoute.RouteItems[0];
        Assert.NotEqual(Guid.Empty, firstItem.Id);
        Assert.NotEqual(Guid.Empty, firstItem.MailboxId);
        Assert.NotNull(firstItem.Address);
        Assert.True(firstItem.Latitude > 0);
        Assert.True(firstItem.Longitude > 0);
        Assert.Equal(1, firstItem.Order);
        Assert.NotNull(firstItem.Priority);
        Assert.NotNull(firstItem.Status);
    }

    [Fact]
    public async Task GetMyAssignedRouteForToday_ShouldReturnOkWithMessage_WhenNoRouteAssignedToday()
    {
        // Arrange — service vraća null (ruta nije dodijeljena ili nije u pravom statusu)
        var postmanId = Guid.Parse(_sut.ControllerContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        _routeServiceMock
            .Setup(service => service.GetPostmanAssignedRouteForTodayAsync(postmanId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((RouteResponse?)null);

        // Act
        var result = await _sut.GetMyAssignedRouteForToday() as OkObjectResult;

        // Assert — controller vraća 200 sa porukom, ne 404
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);

        // Odgovor je anonimni objekat sa "message" poljem, ne RouteResponse
        var value = result.Value;
        Assert.NotNull(value);
        Assert.IsNotType<RouteResponse>(value);

        var message = value!.GetType().GetProperty("Message")?.GetValue(value) as string;
        Assert.NotNull(message);
        Assert.Contains("rute", message, StringComparison.OrdinalIgnoreCase);
    }
}
