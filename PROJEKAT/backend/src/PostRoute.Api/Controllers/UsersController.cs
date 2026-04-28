using Microsoft.AspNetCore.Mvc;
using PostRoute.Api.Contracts.Users;
using PostRoute.BLL.Commands;
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

        var response = new UserResponse(
    user.Id,
    user.Username,
    user.Email,
    user.Role,
    user.MustChangePassword
);
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<UserResponse>> CreateAsync(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new CreateUserCommand(
                request.FirstName,
                request.LastName,
                request.Username,
                request.Email,
                request.Password
            );

            var user = await _userService.CreateAsync(command, cancellationToken);
            var response = new UserResponse(
    user.Id,
    user.Username,
    user.Email,
    user.Role,
    user.MustChangePassword
);

            return Created($"/api/users/{user.Id}", response);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
    [HttpPost("login")]
    public async Task<ActionResult<UserResponse>> LoginAsync(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userService.LoginAsync(request.Email, request.Password, cancellationToken);
            var response = new UserResponse(
    user.Id,
    user.Username,
    user.Email,
    user.Role,
    user.MustChangePassword
);
            return Ok(response);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("locked"))
        {
            return StatusCode(423);
        }
        catch (InvalidOperationException)
        {
            return BadRequest(new { message = "Invalid credentials" });
        }
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePasswordAsync(
    [FromBody] ChangePasswordRequest request,
    CancellationToken cancellationToken) {
        await _userService.ChangePasswordAsync(
            request.Email,
            request.NewPassword,
            cancellationToken
        );

    return Ok(new { message = "Password changed." });
    }
}
