using CourseHub.Application.DTOs.Enrollments;

namespace CourseHub.Application.Interfaces.Services;

public interface IEnrollmentService
{
    Task<EnrollmentDto> EnrollAsync(int courseId, int userId);
    Task<List<EnrollmentDto>> GetMyEnrollmentsAsync(int userId);
}