using Microsoft.AspNetCore.Mvc;
using Moq;
using PostRoute.Api.Controllers;
using PostRoute.BLL.Models.Routes;
using PostRoute.BLL.Services;
using PostRoute.DAL.Entities;
using Xunit;

namespace PostRoute.Api.Tests.Controllers;

public sealed class RoutesControllerTestsPBI024
{
    private readonly Mock<IRouteService> _routeServiceMock;
    private readonly RoutesController _sut;

    public RoutesControllerTestsPBI024()
    {
        _routeServiceMock = new Mock<IRouteService>();
        _sut = new RoutesController(_routeServiceMock.Object);
    }

    [Fact]
    public async Task GetRouteDetails_ShouldReturnOkResult_WithRouteData()
    {
        // Arrange
        var routeId = Guid.NewGuid();
        var postmanId = Guid.NewGuid();
        var mailboxId = Guid.NewGuid();

        var routeResponse = new RouteResponse
        {
            Id = routeId,
            PostmanId = postmanId,
            Date = new DateOnly(2026, 5, 19),
            PlannedStartTime = new TimeOnly(8, 0),
            PlannedEndTime = new TimeOnly(12, 0),
            TotalDistanceKm = 15.5m,
            TotalDurationMinutes = 240,
            RouteItems = new List<RouteItemResponse>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Order = 1,
                    MailboxId = mailboxId,
                    Address = "Sarajevo, Test 1",
                    Latitude = 43.85m,
                    Longitude = 18.41m,
                    Priority = MailboxPriority.Visok.ToString(),
                    EstimatedArrivalTime = new TimeOnly(8, 15),
                    Status = "Planirano"
                }
            },
            TotalMailboxesCount = 100,
            ActiveMailboxesCount = 95,
            DayFilteredMailboxesCount = 80
        };

        _routeServiceMock
            .Setup(x => x.GetRouteDetailsAsync(routeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(routeResponse);

        // Act
        var result = await _sut.GetRouteDetails(routeId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);

        var returnedRoute = Assert.IsType<RouteResponse>(okResult.Value);
        Assert.Equal(routeId, returnedRoute.Id);
        Assert.Equal(postmanId, returnedRoute.PostmanId);
        Assert.Equal(new DateOnly(2026, 5, 19), returnedRoute.Date);
        Assert.Single(returnedRoute.RouteItems);
    }

    [Fact]
    public async Task GetRouteDetails_ShouldReturnNotFound_WhenRouteDoesNotExist()
    {
        // Arrange
        var routeId = Guid.NewGuid();

        _routeServiceMock
            .Setup(x => x.GetRouteDetailsAsync(routeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((RouteResponse?)null);

        // Act
        var result = await _sut.GetRouteDetails(routeId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(404, notFoundResult.StatusCode);

        Assert.Contains("Ruta nije", notFoundResult.Value?.ToString());
    }

    [Fact]
    public async Task GetRouteDetails_ShouldCallServiceWithCorrectId()
    {
        // Arrange
        var routeId = Guid.NewGuid();

        _routeServiceMock
            .Setup(x => x.GetRouteDetailsAsync(routeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((RouteResponse?)null);

        // Act
        await _sut.GetRouteDetails(routeId);

        // Assert
        _routeServiceMock.Verify(
            x => x.GetRouteDetailsAsync(routeId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetRouteDetails_ShouldReturnRouteWithMultipleItems()
    {
        // Arrange
        var routeId = Guid.NewGuid();
        var postmanId = Guid.NewGuid();

        var routeResponse = new RouteResponse
        {
            Id = routeId,
            PostmanId = postmanId,
            Date = new DateOnly(2026, 5, 19),
            PlannedStartTime = new TimeOnly(8, 0),
            PlannedEndTime = new TimeOnly(12, 0),
            TotalDistanceKm = 25.5m,
            TotalDurationMinutes = 240,
            RouteItems = new List<RouteItemResponse>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Order = 1,
                    MailboxId = Guid.NewGuid(),
                    Address = "Sarajevo, Test 1",
                    Latitude = 43.85m,
                    Longitude = 18.41m,
                    Priority = MailboxPriority.Visok.ToString(),
                    EstimatedArrivalTime = new TimeOnly(8, 15),
                    Status = "Planirano"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Order = 2,
                    MailboxId = Guid.NewGuid(),
                    Address = "Sarajevo, Test 2",
                    Latitude = 43.86m,
                    Longitude = 18.42m,
                    Priority = MailboxPriority.Srednji.ToString(),
                    EstimatedArrivalTime = new TimeOnly(8, 45),
                    Status = "Planirano"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Order = 3,
                    MailboxId = Guid.NewGuid(),
                    Address = "Sarajevo, Test 3",
                    Latitude = 43.87m,
                    Longitude = 18.43m,
                    Priority = MailboxPriority.Nizak.ToString(),
                    EstimatedArrivalTime = new TimeOnly(9, 30),
                    Status = "Planirano"
                }
            },
            TotalMailboxesCount = 150,
            ActiveMailboxesCount = 140,
            DayFilteredMailboxesCount = 120
        };

        _routeServiceMock
            .Setup(x => x.GetRouteDetailsAsync(routeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(routeResponse);

        // Act
        var result = await _sut.GetRouteDetails(routeId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedRoute = Assert.IsType<RouteResponse>(okResult.Value);
        Assert.Equal(3, returnedRoute.RouteItems.Count);
        Assert.Equal("Sarajevo, Test 1", returnedRoute.RouteItems[0].Address);
        Assert.Equal("Sarajevo, Test 2", returnedRoute.RouteItems[1].Address);
        Assert.Equal("Sarajevo, Test 3", returnedRoute.RouteItems[2].Address);
    }

    [Fact]
    public async Task GetRouteDetails_ShouldReturnCorrectTimings()
    {
        // Arrange
        var routeId = Guid.NewGuid();

        var routeResponse = new RouteResponse
        {
            Id = routeId,
            PostmanId = Guid.NewGuid(),
            Date = new DateOnly(2026, 5, 19),
            PlannedStartTime = new TimeOnly(7, 30),
            PlannedEndTime = new TimeOnly(16, 45),
            TotalDistanceKm = 42.3m,
            TotalDurationMinutes = 555,
            RouteItems = new List<RouteItemResponse>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Order = 1,
                    MailboxId = Guid.NewGuid(),
                    Address = "Test",
                    Latitude = 43.85m,
                    Longitude = 18.41m,
                    Priority = MailboxPriority.Visok.ToString(),
                    EstimatedArrivalTime = new TimeOnly(7, 50),
                    Status = "Planirano"
                }
            }
        };

        _routeServiceMock
            .Setup(x => x.GetRouteDetailsAsync(routeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(routeResponse);

        // Act
        var result = await _sut.GetRouteDetails(routeId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedRoute = Assert.IsType<RouteResponse>(okResult.Value);
        Assert.Equal(new TimeOnly(7, 30), returnedRoute.PlannedStartTime);
        Assert.Equal(new TimeOnly(16, 45), returnedRoute.PlannedEndTime);
        Assert.Equal(555, returnedRoute.TotalDurationMinutes);
    }
}
