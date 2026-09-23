using CourseHub.Application.DTOs.Enrollments;
using CourseHub.Domain.Entities;

namespace CourseHub.Application.Interfaces.Repositories;

public interface IEnrollmentRepository : IRepository<Enrollment>
{
    Task<bool> ExistsAsync(int userId, int courseId);
    Task<List<EnrollmentDto>> GetMyEnrollmentsAsync(int userId);
}