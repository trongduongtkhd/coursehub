using CourseHub.Application.DTOs.Reviews;
using CourseHub.Domain.Entities;

namespace CourseHub.Application.Interfaces.Repositories;
public interface IReviewRepository : IRepository<Review>
{
    Task<List<ReviewDto>> GetByCourseAsync(int courseId);
    Task<Review?> GetByUserAndCourseAsync(int userId, int courseId);
    Task<CourseRatingSummaryDto> GetSummaryAsync(int courseId);
}