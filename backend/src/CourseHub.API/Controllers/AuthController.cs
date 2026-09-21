using CourseHub.Application.DTOs.Auth;
using CourseHub.Application.Exceptions;
using CourseHub.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
namespace CourseHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterRequest request)
    {
        try
        {
            var user = await _authService.RegisterAsync(request);
            return StatusCode(201, user);
        }
        catch (ConflictException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
{
    try
    {
        var result = await _authService.LoginAsync(request);
        return Ok(result);
    }
    catch (UnauthorizedException ex)
    {
        return Unauthorized(new { message = ex.Message });
    }
}

[Authorize]
[HttpGet("me")]
public ActionResult Me()
{
    var claims = User.Claims.Select(c => new { c.Type, c.Value });
    return Ok(claims);
}
}