using CourseHub.Application.DTOs.Auth;

namespace CourseHub.Application.Interfaces.Services;

public interface IAuthService
{
    Task<UserDto> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
}