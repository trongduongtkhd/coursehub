using CourseHub.Application.DTOs.Reviews;
using CourseHub.Application.Interfaces.Repositories;
using CourseHub.Domain.Entities;
using CourseHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CourseHub.Infrastructure.Repositories;

public class ReviewRepository : Repository<Review>, IReviewRepository
{
    public ReviewRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<ReviewDto>> GetByCourseAsync(int courseId)
    {
        return await DbSet
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

    public async Task<Review?> GetByUserAndCourseAsync(int userId, int courseId)
    {
        return await DbSet.FirstOrDefaultAsync(r => r.CourseId == courseId && r.UserId == userId);
    }

    public async Task<CourseRatingSummaryDto> GetSummaryAsync(int courseId)
    {
        var query = DbSet.Where(r => r.CourseId == courseId);
        var count = await query.CountAsync();

        return new CourseRatingSummaryDto
        {
            AverageRating = count == 0 ? 0 : Math.Round(await query.AverageAsync(r => r.Rating), 1),
            ReviewCount = count
        };
    }
}