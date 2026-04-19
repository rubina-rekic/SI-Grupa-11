using Microsoft.AspNetCore.Mvc;
using PostRoute.Api.Contracts.Users;
using PostRoute.BLL.Services;

namespace PostRoute.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{userId:guid}")]
    public async Task<ActionResult<UserResponse>> GetByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return NotFound();
        }

        var response = new UserResponse(user.Id, user.Username, user.Email, user.Role);
        return Ok(response);
    }
}
