using CourseHub.Application.DTOs.Enrollments;
using CourseHub.Application.Interfaces.Repositories;
using CourseHub.Domain.Entities;
using CourseHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CourseHub.Infrastructure.Repositories;

public class EnrollmentRepository : Repository<Enrollment>, IEnrollmentRepository
{
    public EnrollmentRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsAsync(int userId, int courseId)
    {
        return await DbSet.AnyAsync(e => e.UserId == userId && e.CourseId == courseId);
    }

    public async Task<List<EnrollmentDto>> GetMyEnrollmentsAsync(int userId)
    {
        return await DbSet
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.EnrolledAt)
            .Select(e => new EnrollmentDto
            {
                Id = e.Id,
                CourseId = e.CourseId,
                CourseTitle = e.Course.Title,
                CourseThumbnailUrl = e.Course.ThumbnailUrl,
                InstructorName = e.Course.Instructor.FullName,
                EnrolledAt = e.EnrolledAt,
                TotalLessons = e.Course.Lessons.Count(),
                CompletedLessons = e.Course.Lessons.Count(l => l.Progresses.Any(p => p.UserId == userId && p.IsCompleted))
            })
            .ToListAsync();
    }
}