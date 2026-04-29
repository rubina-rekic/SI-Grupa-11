using Microsoft.AspNetCore.Mvc;
using PostRoute.Api.Contracts.Users;
using PostRoute.Api.Middleware;
using PostRoute.BLL.Commands;
using PostRoute.BLL.Services;
using PostRoute.Domain.Entities;

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
    [RequiredRole("Administrator")]
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
                request.Password,
                request.Role ?? UserRole.PostalWorker
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
            
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("UserRole", user.Role);
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Email", user.Email);
            
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
        CancellationToken cancellationToken)
    {
        var sessionEmail = HttpContext.Session.GetString("Email");

        if (string.IsNullOrEmpty(sessionEmail))
        {
            return Unauthorized(new { message = "Niste prijavljeni." });
        }

        if (!string.Equals(sessionEmail, request.Email, StringComparison.OrdinalIgnoreCase))
        {
            return Forbid();
        }

        try
        {
            await _userService.ChangePasswordAsync(
                request.Email,
                request.CurrentPassword,
                request.NewPassword,
                cancellationToken
            );

            return Ok(new { message = "Password changed." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return Ok(new { message = "Logged out successfully" });
    }

    [HttpGet("current-user")]
    public ActionResult<UserResponse> GetCurrentUser()
    {
        var userId = HttpContext.Session.GetString("UserId");
        var userRole = HttpContext.Session.GetString("UserRole");
        var username = HttpContext.Session.GetString("Username");
        var email = HttpContext.Session.GetString("Email");

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userRole))
        {
            return Unauthorized(new { message = "Not logged in" });
        }

        var response = new UserResponse(
            Guid.Parse(userId),
            username ?? string.Empty,
            email ?? string.Empty,
            userRole,
            false
        );

        return Ok(response);
    }
}
