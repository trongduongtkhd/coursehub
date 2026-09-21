using CourseHub.Application.Interfaces.Services;
using CourseHub.Domain.Entities;
using CourseHub.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CourseHub.Infrastructure.Persistence.Seed;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context, IPasswordHasher passwordHasher)
    {
        var seedUsers = new[]
        {
            new User
            {
                FullName = "Admin CourseHub",
                Email = "admin@coursehub.com",
                PasswordHash = passwordHasher.Hash("Admin@123"),
                Role = Role.Admin
            },
            new User
            {
                FullName = "Nguyen Van Instructor",
                Email = "instructor1@coursehub.com",
                PasswordHash = passwordHasher.Hash("Instructor@123"),
                Role = Role.Instructor
            },
            new User
            {
                FullName = "Tran Thi Instructor",
                Email = "instructor2@coursehub.com",
                PasswordHash = passwordHasher.Hash("Instructor@123"),
                Role = Role.Instructor
            }
        };

        var existingEmails = await context.Users
            .Where(u => seedUsers.Select(s => s.Email).Contains(u.Email))
            .Select(u => u.Email)
            .ToListAsync();

        var usersToAdd = seedUsers.Where(u => !existingEmails.Contains(u.Email)).ToList();
        if (usersToAdd.Count == 0)
        {
            return;
        }

        context.Users.AddRange(usersToAdd);
        await context.SaveChangesAsync();
    }
}