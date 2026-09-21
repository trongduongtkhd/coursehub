using CourseHub.Application.DTOs.Enrollments;
using CourseHub.Application.Exceptions;
using CourseHub.Application.Interfaces;
using CourseHub.Application.Interfaces.Services;
using CourseHub.Domain.Entities;
using CourseHub.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CourseHub.Application.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly IAppDbContext _context;

    public EnrollmentService(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<EnrollmentDto> EnrollAsync(int courseId, int userId)
    {
        var course = await _context.Courses.Include(c => c.Instructor).FirstOrDefaultAsync(c => c.Id == courseId);
        if (course == null)
        {
            throw new NotFoundException("Không tìm thấy khóa học.");
        }

        if (course.Status != CourseStatus.Published)
        {
            throw new BadRequestException("Khóa học chưa được xuất bản, không thể đăng ký.");
        }

        if (course.InstructorId == userId)
        {
            throw new BadRequestException("Bạn không thể đăng ký khóa học do chính mình giảng dạy.");
        }

        var alreadyEnrolled = await _context.Enrollments.AnyAsync(e => e.UserId == userId && e.CourseId == courseId);
        if (alreadyEnrolled)
        {
            throw new ConflictException("Bạn đã đăng ký khóa học này rồi.");
        }

        var enrollment = new Enrollment
        {
            UserId = userId,
            CourseId = courseId,
            EnrolledAt = DateTime.UtcNow
        };

        _context.Enrollments.Add(enrollment);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new ConflictException("Bạn đã đăng ký khóa học này rồi.");
        }

        return new EnrollmentDto
        {
            Id = enrollment.Id,
            CourseId = course.Id,
            CourseTitle = course.Title,
            CourseThumbnailUrl = course.ThumbnailUrl,
            InstructorName = course.Instructor.FullName,
            EnrolledAt = enrollment.EnrolledAt
        };
    }

public async Task<List<EnrollmentDto>> GetMyEnrollmentsAsync(int userId)
{
    return await _context.Enrollments
        .Where(e => e.UserId == userId)
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
        .OrderByDescending(e => e.EnrolledAt)
        .ToListAsync();
}
}