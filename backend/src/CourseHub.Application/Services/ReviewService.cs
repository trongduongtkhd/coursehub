using CourseHub.Application.DTOs.Reviews;
using CourseHub.Application.Exceptions;
using CourseHub.Application.Interfaces;
using CourseHub.Application.Interfaces.Services;
using CourseHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CourseHub.Application.Services;

public class ReviewService : IReviewService
{
    private readonly IAppDbContext _context;

    public ReviewService(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ReviewDto>> GetByCourseAsync(int courseId)
    {
        return await _context.Reviews
            .Where(r => r.CourseId == courseId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReviewDto
            {
                Id = r.Id,
                CourseId = r.CourseId,
                UserId = r.UserId,
                UserName = r.User.FullName,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<ReviewDto> UpsertAsync(int courseId, CreateReviewRequest request, int userId)
    {
    

        var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);
        if (course == null)
        {
            throw new NotFoundException("Không tìm thấy khóa học.");
        }

        var isEnrolled = await _context.Enrollments.AnyAsync(e => e.CourseId == courseId && e.UserId == userId);
        if (!isEnrolled)
        {
            throw new ForbiddenException("Bạn cần đăng ký khóa học trước khi đánh giá.");
        }

        var review = await _context.Reviews.FirstOrDefaultAsync(r => r.CourseId == courseId && r.UserId == userId);

        if (review == null)
        {
            review = new Review { CourseId = courseId, UserId = userId, Rating = request.Rating, Comment = request.Comment };
            _context.Reviews.Add(review);
        }
        else
        {
            review.Rating = request.Rating;
            review.Comment = request.Comment;
            review.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        var user = await _context.Users.FirstAsync(u => u.Id == userId);

        return new ReviewDto
        {
            Id = review.Id,
            CourseId = review.CourseId,
            UserId = review.UserId,
            UserName = user.FullName,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt
        };
    }

    public async Task<CourseRatingSummaryDto> GetRatingSummaryAsync(int courseId)
    {
        var query = _context.Reviews.Where(r => r.CourseId == courseId);
        var count = await query.CountAsync();

        return new CourseRatingSummaryDto
        {
            AverageRating = count == 0 ? 0 : Math.Round(await query.AverageAsync(r => r.Rating), 1),
            ReviewCount = count
        };
    }
}