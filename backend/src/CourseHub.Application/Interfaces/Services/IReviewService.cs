using CourseHub.Application.DTOs.Reviews;

namespace CourseHub.Application.Interfaces.Services;

public interface IReviewService
{
    Task<List<ReviewDto>> GetByCourseAsync(int courseId);
    Task<ReviewDto> UpsertAsync(int courseId, CreateReviewRequest request, int userId);
    Task<CourseRatingSummaryDto> GetRatingSummaryAsync(int courseId);
}