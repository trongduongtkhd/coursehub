using CourseHub.Application.DTOs.Enrollments;
using CourseHub.Application.Exceptions;
using CourseHub.Application.Interfaces;
using CourseHub.Application.Interfaces.Services;
using CourseHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CourseHub.Application.Services;

public class ProgressService : IProgressService
{
    private readonly IAppDbContext _context;

    public ProgressService(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<LessonProgressDto>> GetCourseProgressAsync(int courseId, int userId)
    {
        var isEnrolled = await _context.Enrollments.AnyAsync(e => e.CourseId == courseId && e.UserId == userId);
        if (!isEnrolled)
        {
            throw new ForbiddenException("Bạn chưa đăng ký khóa học này.");
        }

        return await _context.Lessons
            .Where(l => l.CourseId == courseId)
            .OrderBy(l => l.OrderIndex)
            .Select(l => new LessonProgressDto
            {
                LessonId = l.Id,
                Title = l.Title,
                Content = l.Content,
                OrderIndex = l.OrderIndex,
                IsCompleted = l.Progresses.Any(p => p.UserId == userId && p.IsCompleted)
            })
            .ToListAsync();
    }

    public async Task MarkCompleteAsync(int lessonId, int userId)
    {
        var lesson = await _context.Lessons.FirstOrDefaultAsync(l => l.Id == lessonId);
        if (lesson == null)
        {
            throw new NotFoundException("Không tìm thấy bài học.");
        }

        var isEnrolled = await _context.Enrollments.AnyAsync(e => e.CourseId == lesson.CourseId && e.UserId == userId);
        if (!isEnrolled)
        {
            throw new ForbiddenException("Bạn chưa đăng ký khóa học chứa bài học này.");
        }

        var progress = await _context.LessonProgresses.FirstOrDefaultAsync(p => p.LessonId == lessonId && p.UserId == userId);

        if (progress != null)
        {
            if (!progress.IsCompleted)
            {
                progress.IsCompleted = true;
                progress.CompletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
            return;
        }

        _context.LessonProgresses.Add(new LessonProgress
        {
            LessonId = lessonId,
            UserId = userId,
            IsCompleted = true,
            CompletedAt = DateTime.UtcNow
        });

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            // Race condition: request khác đã insert trước — kết quả cuối cùng vẫn là "đã hoàn thành", bỏ qua lỗi.
        }
    }

    public async Task UnmarkCompleteAsync(int lessonId, int userId)
    {
        var progress = await _context.LessonProgresses.FirstOrDefaultAsync(p => p.LessonId == lessonId && p.UserId == userId);
        if (progress != null)
        {
            progress.IsCompleted = false;
            progress.CompletedAt = null;
            await _context.SaveChangesAsync();
        }
    }
}