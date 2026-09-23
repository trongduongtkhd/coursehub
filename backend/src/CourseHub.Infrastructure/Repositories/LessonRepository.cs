using CourseHub.Application.Interfaces.Repositories;
using CourseHub.Domain.Entities;
using CourseHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using CourseHub.Application.DTOs.Enrollments;
namespace CourseHub.Infrastructure.Repositories;

public class LessonRepository : Repository<Lesson>, ILessonRepository
{
    public LessonRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<Lesson>> GetByCourseAsync(int courseId)
    {
        return await DbSet.Where(l => l.CourseId == courseId).OrderBy(l => l.OrderIndex).ToListAsync();
    }

    public async Task<int> GetMaxOrderIndexAsync(int courseId)
    {
        return await DbSet.Where(l => l.CourseId == courseId).Select(l => (int?)l.OrderIndex).MaxAsync() ?? 0;
    }

    public async Task<Lesson?> GetWithCourseAsync(int lessonId)
    {
        return await DbSet.Include(l => l.Course).FirstOrDefaultAsync(l => l.Id == lessonId);
    }

    public async Task<List<LessonProgressDto>> GetProgressByCourseAsync(int courseId, int userId)
{
    return await DbSet
        .Where(l => l.CourseId == courseId)
        .OrderBy(l => l.OrderIndex)
        .Select(l => new LessonProgressDto
        {
            LessonId = l.Id,
            Title = l.Title,
            Content = l.Content,
            OrderIndex = l.OrderIndex,
            IsCompleted = l.Progresses.Any(p => p.UserId == userId && p.IsCompleted)
        })
        .ToListAsync();
}
}