using CourseHub.Domain.Entities;
using CourseHub.Application.DTOs.Enrollments;
namespace CourseHub.Application.Interfaces.Repositories;

public interface ILessonRepository : IRepository<Lesson>
{
    Task<List<Lesson>> GetByCourseAsync(int courseId);
    Task<int> GetMaxOrderIndexAsync(int courseId);
    Task<Lesson?> GetWithCourseAsync(int lessonId);
    Task<List<LessonProgressDto>> GetProgressByCourseAsync(int courseId, int userId);
}