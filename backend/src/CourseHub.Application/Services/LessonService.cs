using CourseHub.Application.DTOs.Lessons;
using CourseHub.Application.Exceptions;
using CourseHub.Application.Interfaces;
using CourseHub.Application.Interfaces.Services;
using CourseHub.Domain.Entities;
using CourseHub.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CourseHub.Application.Services;

public class LessonService : ILessonService
{
    private readonly IAppDbContext _context;

    public LessonService(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<LessonDto>> GetByCourseAsync(int courseId)
    {
        return await _context.Lessons
            .Where(l => l.CourseId == courseId)
            .OrderBy(l => l.OrderIndex)
            .Select(l => new LessonDto
            {
                Id = l.Id,
                Title = l.Title,
                Content = l.Content,
                OrderIndex = l.OrderIndex,
                CourseId = l.CourseId
            })
            .ToListAsync();
    }

    public async Task<LessonDto> CreateAsync(int courseId, CreateLessonRequest request, int currentUserId, string currentUserRole)
    {
        var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);
        if (course == null)
        {
            throw new NotFoundException("Không tìm thấy khóa học.");
        }

        EnsureCanModify(course, currentUserId, currentUserRole);

        var maxOrder = await _context.Lessons
            .Where(l => l.CourseId == courseId)
            .Select(l => (int?)l.OrderIndex)
            .MaxAsync() ?? 0;

        var lesson = new Lesson
        {
            CourseId = courseId,
            Title = request.Title,
            Content = request.Content,
            OrderIndex = maxOrder + 1
        };

        _context.Lessons.Add(lesson);
        await _context.SaveChangesAsync();

        return new LessonDto
        {
            Id = lesson.Id,
            Title = lesson.Title,
            Content = lesson.Content,
            OrderIndex = lesson.OrderIndex,
            CourseId = lesson.CourseId
        };
    }

    public async Task UpdateAsync(int lessonId, UpdateLessonRequest request, int currentUserId, string currentUserRole)
    {
        var lesson = await _context.Lessons.Include(l => l.Course).FirstOrDefaultAsync(l => l.Id == lessonId);
        if (lesson == null)
        {
            throw new NotFoundException("Không tìm thấy bài học.");
        }

        EnsureCanModify(lesson.Course, currentUserId, currentUserRole);

        lesson.Title = request.Title;
        lesson.Content = request.Content;
        lesson.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int lessonId, int currentUserId, string currentUserRole)
    {
        var lesson = await _context.Lessons.Include(l => l.Course).FirstOrDefaultAsync(l => l.Id == lessonId);
        if (lesson == null)
        {
            throw new NotFoundException("Không tìm thấy bài học.");
        }

        EnsureCanModify(lesson.Course, currentUserId, currentUserRole);

        _context.Lessons.Remove(lesson);
        await _context.SaveChangesAsync();
    }

    private static void EnsureCanModify(Course course, int currentUserId, string currentUserRole)
    {
        var isOwner = course.InstructorId == currentUserId;
        var isAdmin = currentUserRole == Role.Admin.ToString();

        if (!isOwner && !isAdmin)
        {
            throw new ForbiddenException("Bạn không có quyền chỉnh sửa bài học của khóa học này.");
        }
    }
}