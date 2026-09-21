using CourseHub.Application.DTOs.Lessons;

namespace CourseHub.Application.Interfaces.Services;

public interface ILessonService
{
    Task<List<LessonDto>> GetByCourseAsync(int courseId);
    Task<LessonDto> CreateAsync(int courseId, CreateLessonRequest request, int currentUserId, string currentUserRole);
    Task UpdateAsync(int lessonId, UpdateLessonRequest request, int currentUserId, string currentUserRole);
    Task DeleteAsync(int lessonId, int currentUserId, string currentUserRole);
}