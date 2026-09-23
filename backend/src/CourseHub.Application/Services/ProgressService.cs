using CourseHub.Application.DTOs.Enrollments;
using CourseHub.Application.Exceptions;
using CourseHub.Application.Interfaces.Repositories;
using CourseHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using CourseHub.Application.Interfaces.Services;
namespace CourseHub.Application.Services;

public class ProgressService : IProgressService
{
    private readonly IUnitOfWork _unitOfWork;

    public ProgressService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<LessonProgressDto>> GetCourseProgressAsync(int courseId, int userId)
    {
        var isEnrolled = await _unitOfWork.Enrollments.ExistsAsync(userId, courseId);
        if (!isEnrolled)
        {
            throw new ForbiddenException("Bạn chưa đăng ký khóa học này.");
        }

        return await _unitOfWork.Lessons.GetProgressByCourseAsync(courseId, userId);
    }

    public async Task MarkCompleteAsync(int lessonId, int userId)
    {
        var lesson = await _unitOfWork.Lessons.GetByIdAsync(lessonId);
        if (lesson == null)
        {
            throw new NotFoundException("Không tìm thấy bài học.");
        }

        var isEnrolled = await _unitOfWork.Enrollments.ExistsAsync(userId, lesson.CourseId);
        if (!isEnrolled)
        {
            throw new ForbiddenException("Bạn chưa đăng ký khóa học chứa bài học này.");
        }

        var progress = await _unitOfWork.LessonProgresses.GetAsync(lessonId, userId);

        if (progress != null)
        {
            if (!progress.IsCompleted)
            {
                progress.IsCompleted = true;
                progress.CompletedAt = DateTime.UtcNow;
                _unitOfWork.LessonProgresses.Update(progress);
                await _unitOfWork.SaveChangesAsync();
            }
            return;
        }

        await _unitOfWork.LessonProgresses.AddAsync(new LessonProgress
        {
            LessonId = lessonId,
            UserId = userId,
            IsCompleted = true,
            CompletedAt = DateTime.UtcNow
        });

        try
        {
            await _unitOfWork.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            // Race condition: request khác đã insert trước — kết quả cuối cùng vẫn "đã hoàn thành", bỏ qua lỗi.
        }
    }

    public async Task UnmarkCompleteAsync(int lessonId, int userId)
    {
        var progress = await _unitOfWork.LessonProgresses.GetAsync(lessonId, userId);
        if (progress != null)
        {
            progress.IsCompleted = false;
            progress.CompletedAt = null;
            _unitOfWork.LessonProgresses.Update(progress);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}