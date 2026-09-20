using CourseHub.Application.DTOs.Auth;
using CourseHub.Application.Exceptions;
using CourseHub.Application.Interfaces;
using CourseHub.Application.Interfaces.Services;
using CourseHub.Domain.Entities;
using CourseHub.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CourseHub.Application.Services;

public class AuthService : IAuthService
{
    private readonly IAppDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(IAppDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
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
}