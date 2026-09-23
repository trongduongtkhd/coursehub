using CourseHub.Domain.Entities;

namespace CourseHub.Application.Interfaces.Repositories;

public interface IUserRepository : IRepository<User>
{
     Task<User?> GetByEmailAsync(string email);
}