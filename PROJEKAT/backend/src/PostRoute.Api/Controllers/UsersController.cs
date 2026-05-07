using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using PostRoute.Api.Contracts.Users;
using PostRoute.Api.Middleware;
using PostRoute.BLL.Commands;
using PostRoute.BLL.Exceptions;
using PostRoute.BLL.Services;
using PostRoute.DAL.Entities;
using PostRoute.DAL.Repositories;
using PostRoute.Domain.Entities;
using System.Security.Claims;

namespace PostRoute.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ISecurityLogRepository _securityLogRepository;

    public UsersController(IUserService userService, ISecurityLogRepository securityLogRepository)
    {
        _userService = userService;
        _securityLogRepository = securityLogRepository;
    }

    private async Task LogLoginAttemptAsync(
        Guid? userId,
        string? userRole,
        string accessType,
        bool isSuccessful,
        CancellationToken cancellationToken)
    {
        var log = new SecurityLog
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            AttemptedUrl = Request.Path + Request.QueryString,
            UserRole = userRole,
            IpAddress = Request.Headers["X-Forwarded-For"].FirstOrDefault()?.Split(',')[0].Trim()
                ?? HttpContext.Connection.RemoteIpAddress?.ToString()
                ?? "Unknown",
            AccessType = accessType,
            UserAgent = Request.Headers["User-Agent"].ToString(),
            IsSuccessful = isSuccessful,
        };

        await _securityLogRepository.AddAsync(log, cancellationToken);
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
            user.MustChangePassword,
            user.IsLockedOut
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
                user.MustChangePassword,
                user.IsLockedOut
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
            Console.WriteLine("=== LOGIN DEBUG ===");
            Console.WriteLine($"Login attempt for email: {request.Email}");

            var user = await _userService.LoginAsync(request.Email, request.Password, cancellationToken);

            Console.WriteLine($"User found: {user.Id}, Role: {user.Role}");
            Console.WriteLine($"Session ID before: {HttpContext.Session.Id}");

            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("UserRole", user.Role);
            HttpContext.Session.SetString("Email", user.Email);
            HttpContext.Session.SetString("MustChangePassword", user.MustChangePassword.ToString());

            Console.WriteLine($"Session ID after: {HttpContext.Session.Id}");
            Console.WriteLine($"Session UserId: {HttpContext.Session.GetString("UserId")}");
            Console.WriteLine($"Session UserRole: {HttpContext.Session.GetString("UserRole")}");

            // Create cookie authentication
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, 
                new ClaimsPrincipal(claimsIdentity), authProperties);

            await LogLoginAttemptAsync(user.Id, user.Role, "LoginSuccess", true, cancellationToken);

            var response = new UserResponse(
                user.Id,
                user.Username,
                user.Email,
                user.Role,
                user.MustChangePassword,
                user.IsLockedOut
            );
            return Ok(response);
        }
        catch (AccountLockedException)
        {
            await LogLoginAttemptAsync(null, null, "AccountLocked", false, cancellationToken);
            return StatusCode(423, new { message = "Račun je zaključan nakon više neuspješnih pokušaja." });
        }
        catch (InvalidCredentialsException)
        {
            await LogLoginAttemptAsync(null, null, "LoginFailed", false, cancellationToken);
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

            HttpContext.Session.SetString("MustChangePassword", "False");

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
        Console.WriteLine("=== CURRENT USER DEBUG ===");
        Console.WriteLine($"Session ID: {HttpContext.Session.Id}");
        Console.WriteLine($"Session UserId: {HttpContext.Session.GetString("UserId")}");
        Console.WriteLine($"Session UserRole: {HttpContext.Session.GetString("UserRole")}");
        Console.WriteLine($"Session Email: {HttpContext.Session.GetString("Email")}");
        Console.WriteLine($"Available session keys: {string.Join(", ", HttpContext.Session.Keys)}");

        var userId = HttpContext.Session.GetString("UserId");
        var userRole = HttpContext.Session.GetString("UserRole");
        var username = HttpContext.Session.GetString("Username");
        var email = HttpContext.Session.GetString("Email");

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userRole))
        {
            Console.WriteLine("User not logged in - returning 401");
            return Unauthorized(new { message = "Not logged in" });
        }

        var mustChangePasswordStr = HttpContext.Session.GetString("MustChangePassword");
        var mustChangePassword = bool.TryParse(mustChangePasswordStr, out var mcp) && mcp;

        var response = new UserResponse(
            Guid.Parse(userId),
            username ?? string.Empty,
            email ?? string.Empty,
            userRole,
            mustChangePassword,
            false
        );

        return Ok(response);
    }
    [RequiredRole("Administrator")]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var users = await _userService.GetAllAsync(cancellationToken);
        var response = users.Select(u => new UserResponse(u.Id, u.Username, u.Email, u.Role, u.MustChangePassword, u.IsLockedOut));
        return Ok(response);
    }
}
