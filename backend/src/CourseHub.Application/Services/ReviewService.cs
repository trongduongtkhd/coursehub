using CourseHub.Application.DTOs.Reviews;
using CourseHub.Application.Exceptions;
using CourseHub.Application.Interfaces.Repositories;
using CourseHub.Domain.Entities;
using CourseHub.Application.Interfaces.Services;
namespace CourseHub.Application.Services;

public class ReviewService : IReviewService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cache;

    public ReviewService(IUnitOfWork unitOfWork, ICacheService cache)
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    public async Task<List<ReviewDto>> GetByCourseAsync(int courseId)
    {
        return await _unitOfWork.Reviews.GetByCourseAsync(courseId);
    }

    public async Task<ReviewDto> UpsertAsync(int courseId, CreateReviewRequest request, int userId)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(courseId);
        if (course == null)
        {
            throw new NotFoundException("Không tìm thấy khóa học.");
        }

        var isEnrolled = await _unitOfWork.Enrollments.ExistsAsync(userId, courseId);
        if (!isEnrolled)
        {
            throw new ForbiddenException("Bạn cần đăng ký khóa học trước khi đánh giá.");
        }

        var review = await _unitOfWork.Reviews.GetByUserAndCourseAsync(userId, courseId);

        if (review == null)
        {
            review = new Review { CourseId = courseId, UserId = userId, Rating = request.Rating, Comment = request.Comment };
            await _unitOfWork.Reviews.AddAsync(review);
        }
        else
        {
            review.Rating = request.Rating;
            review.Comment = request.Comment;
            review.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Reviews.Update(review);
        }

        await _unitOfWork.SaveChangesAsync();

        // CourseDto chứa AverageRating/ReviewCount nên phải xóa cache chi tiết khóa học
        _cache.Remove($"course:{courseId}");

        var user = await _unitOfWork.Users.GetByIdAsync(userId);

        return new ReviewDto
        {
            Id = review.Id,
            CourseId = review.CourseId,
            UserId = review.UserId,
            UserName = user!.FullName,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt
        };
    }

    public async Task<CourseRatingSummaryDto> GetRatingSummaryAsync(int courseId)
    {
        return await _unitOfWork.Reviews.GetSummaryAsync(courseId);
    }
}