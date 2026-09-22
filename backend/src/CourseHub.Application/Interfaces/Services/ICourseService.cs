using CourseHub.Application.DTOs.Courses;

namespace CourseHub.Application.Interfaces.Services;
using CourseHub.Application.DTOs.Common;
public interface ICourseService
{

    Task<CourseDto> GetByIdAsync(int id);
    Task<CourseDto> CreateAsync(CreateCourseRequest request, int instructorId);
    Task UpdateAsync(int id, UpdateCourseRequest request, int currentUserId, string currentUserRole);
    Task DeleteAsync(int id, int currentUserId, string currentUserRole);
    Task<string> UpdateThumbnailAsync(int courseId, Stream fileStream, string fileExtension, int currentUserId, string currentUserRole);
    Task<PagedResult<CourseDto>> GetAllAsync(CourseQueryParameters query);
}