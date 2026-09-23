using CourseHub.Domain.Entities;

namespace CourseHub.Application.Interfaces.Repositories;

public interface ILessonProgressRepository : IRepository<LessonProgress>
{
    Task<LessonProgress?> GetAsync(int lessonId, int userId);
}