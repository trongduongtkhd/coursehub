using CourseHub.Application.DTOs.Auth;
using CourseHub.Application.Exceptions;
using CourseHub.Application.Interfaces;
using CourseHub.Application.Interfaces.Services;
using CourseHub.Domain.Entities;
using CourseHub.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
namespace CourseHub.Application.Services;

public class AuthService : IAuthService
{
    private readonly IAppDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
private readonly ILogger<AuthService> _logger;
    public AuthService(IAppDbContext context, IPasswordHasher passwordHasher,ITokenService tokenService, ILogger<AuthService> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
          _tokenService = tokenService;
          _logger = logger;
    }

    public async Task<UserDto> RegisterAsync(RegisterRequest request)
    {
        var existing = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (existing != null)
        {
            throw new ConflictException("Email đã được sử dụng.");
        }

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            Role = Role.Student
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role.ToString()
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
{
      var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

    if (user == null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
    {
        _logger.LogWarning("Đăng nhập thất bại cho email {Email}", request.Email);
        throw new UnauthorizedException("Email hoặc mật khẩu không đúng.");
    }

    var accessToken = _tokenService.GenerateAccessToken(user);
    var refreshTokenValue = _tokenService.GenerateRefreshToken();

    _context.RefreshTokens.Add(new RefreshToken
    {
        Token = refreshTokenValue,
        UserId = user.Id,
        ExpiresAt = _tokenService.GetRefreshTokenExpiry()
    });
    await _context.SaveChangesAsync();
   _logger.LogInformation("User {UserId} đăng nhập thành công", user.Id);
    return new AuthResponse
    {
        AccessToken = accessToken,
        RefreshToken = refreshTokenValue,
        User = new UserDto { Id = user.Id, FullName = user.FullName, Email = user.Email, Role = user.Role.ToString() }
    };
}
}