using CourseHub.Application.DTOs.Auth;
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
    var user = await _authService.RegisterAsync(request);
    return StatusCode(201, user);
}

[HttpPost("login")]
public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
{
    var result = await _authService.LoginAsync(request);
    return Ok(result);
}

[Authorize]
[HttpGet("me")]
public ActionResult Me()
{
    var claims = User.Claims.Select(c => new { c.Type, c.Value });
    return Ok(claims);
}
}