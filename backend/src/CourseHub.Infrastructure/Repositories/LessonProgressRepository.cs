using CourseHub.Application.Interfaces.Repositories;
using CourseHub.Domain.Entities;
using CourseHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CourseHub.Infrastructure.Repositories;

public class LessonProgressRepository : Repository<LessonProgress>, ILessonProgressRepository
{
    public LessonProgressRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<LessonProgress?> GetAsync(int lessonId, int userId)
    {
        return await DbSet.FirstOrDefaultAsync(p => p.LessonId == lessonId && p.UserId == userId);
    }
}