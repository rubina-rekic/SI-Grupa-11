using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PostRoute.Api.Controllers;
using PostRoute.BLL.Models.Routes;
using PostRoute.BLL.Services;
using Xunit;

namespace PostRoute.Api.Tests.Controllers;

public sealed class RoutesControllerTestsPBI023
{
    private readonly Mock<IRouteService> _routeServiceMock = new();
    private readonly RoutesController _sut;

    public RoutesControllerTestsPBI023()
    {
        _sut = new RoutesController(_routeServiceMock.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(
                        new[] { new Claim(ClaimTypes.Name, "dispatcher") },
                        "TestAuth"))
                }
            }
        };
    }

    [Fact]
    public async Task Assign_ShouldReturnOk_WithAssignedRoute()
    {
        var routeId = Guid.NewGuid();
        var postmanId = Guid.NewGuid();
        var response = new RouteResponse
        {
            Id = routeId,
            PostmanId = postmanId,
            Status = "Dodijeljena",
            AssignedBy = "dispatcher"
        };

        _routeServiceMock
            .Setup(service => service.AssignRouteAsync(
                routeId,
                It.Is<AssignRouteRequest>(request => request.PostmanId == postmanId),
                "dispatcher",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await _sut.Assign(routeId, new AssignRouteRequest { PostmanId = postmanId });

        var okResult = Assert.IsType<OkObjectResult>(result);
        var assignedRoute = Assert.IsType<RouteResponse>(okResult.Value);
        Assert.Equal("Dodijeljena", assignedRoute.Status);
        Assert.Equal(postmanId, assignedRoute.PostmanId);
    }

    [Fact]
    public async Task Assign_ShouldReturnBadRequest_WhenServiceRejectsAssignment()
    {
        var routeId = Guid.NewGuid();

        _routeServiceMock
            .Setup(service => service.AssignRouteAsync(
                routeId,
                It.IsAny<AssignRouteRequest>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Postar vec ima dodijeljenu rutu za ovaj datum."));

        var result = await _sut.Assign(routeId, new AssignRouteRequest { PostmanId = Guid.NewGuid() });

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequest.Value);
    }

    [Fact]
    public async Task GetAvailablePostmen_ShouldReturnOk_WithAvailabilityList()
    {
        var routeId = Guid.NewGuid();
        var postmanId = Guid.NewGuid();
        var response = new List<AvailablePostmanResponse>
        {
            new()
            {
                Id = postmanId,
                FullName = "Amar Hodzic",
                Email = "amar@postroute.ba",
                Username = "amar.hodzic",
                IsAvailable = true
            }
        };

        _routeServiceMock
            .Setup(service => service.GetAvailablePostmenAsync(routeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await _sut.GetAvailablePostmen(routeId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var postmen = Assert.IsAssignableFrom<IReadOnlyList<AvailablePostmanResponse>>(okResult.Value);
        Assert.Single(postmen);
        Assert.True(postmen[0].IsAvailable);
    }
}
