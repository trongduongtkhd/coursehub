using CourseHub.Application.DTOs.Enrollments;
using CourseHub.Application.Exceptions;
using CourseHub.Application.Interfaces.Repositories;
using CourseHub.Domain.Entities;
using CourseHub.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CourseHub.Application.Interfaces.Services;
namespace CourseHub.Application.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<EnrollmentService> _logger;

    public EnrollmentService(IUnitOfWork unitOfWork, ILogger<EnrollmentService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<EnrollmentDto> EnrollAsync(int courseId, int userId)
    {
        var course = await _unitOfWork.Courses.GetWithDetailsAsync(courseId);
        if (course == null)
        {
            throw new NotFoundException("Không tìm thấy khóa học.");
        }

        if (course.Status != CourseStatus.Published)
        {
            _logger.LogWarning("User {UserId} cố đăng ký khóa học {CourseId} chưa xuất bản", userId, courseId);
            throw new BadRequestException("Khóa học chưa được xuất bản, không thể đăng ký.");
        }

        if (course.InstructorId == userId)
        {
            throw new BadRequestException("Bạn không thể đăng ký khóa học do chính mình giảng dạy.");
        }

        var alreadyEnrolled = await _unitOfWork.Enrollments.ExistsAsync(userId, courseId);
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

        await _unitOfWork.Enrollments.AddAsync(enrollment);

        try
        {
            await _unitOfWork.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new ConflictException("Bạn đã đăng ký khóa học này rồi.");
        }

        _logger.LogInformation("User {UserId} đã đăng ký khóa học {CourseId}", userId, courseId);

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
        return await _unitOfWork.Enrollments.GetMyEnrollmentsAsync(userId);
    }
}