using CourseHub.Application.DTOs.Auth;
using CourseHub.Application.Exceptions;
using CourseHub.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

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
}