using CourseHub.Application.Interfaces.Repositories;
using CourseHub.Domain.Entities;
using CourseHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
namespace CourseHub.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }
    public async Task<User?> GetByEmailAsync(string email)
   {
    return await DbSet.FirstOrDefaultAsync(u => u.Email == email);
   }
}