using CourseHub.Application.DTOs.Courses;

namespace CourseHub.Application.Interfaces.Services;

public interface ICourseService
{
    Task<List<CourseDto>> GetAllAsync();
    Task<CourseDto> GetByIdAsync(int id);
    Task<CourseDto> CreateAsync(CreateCourseRequest request, int instructorId);
    Task UpdateAsync(int id, UpdateCourseRequest request, int currentUserId, string currentUserRole);
    Task DeleteAsync(int id, int currentUserId, string currentUserRole);
}