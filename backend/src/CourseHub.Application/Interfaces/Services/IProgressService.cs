using CourseHub.Application.DTOs.Enrollments;

namespace CourseHub.Application.Interfaces.Services;

public interface IProgressService
{
    Task<List<LessonProgressDto>> GetCourseProgressAsync(int courseId, int userId);
    Task MarkCompleteAsync(int lessonId, int userId);
    Task UnmarkCompleteAsync(int lessonId, int userId);
}