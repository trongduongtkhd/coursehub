using CourseHub.Application.DTOs.Lessons;
using CourseHub.Application.Exceptions;
using CourseHub.Application.Interfaces.Repositories;
using CourseHub.Application.Interfaces.Services;
using CourseHub.Domain.Entities;
using CourseHub.Domain.Enums;

namespace CourseHub.Application.Services;

public class LessonService : ILessonService
{
    private readonly IUnitOfWork _unitOfWork;

    public LessonService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<LessonDto>> GetByCourseAsync(int courseId)
    {
        var lessons = await _unitOfWork.Lessons.GetByCourseAsync(courseId);
        return lessons.Select(ToDto).ToList();
    }

    public async Task<LessonDto> CreateAsync(int courseId, CreateLessonRequest request, int currentUserId, string currentUserRole)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(courseId);
        if (course == null)
        {
            throw new NotFoundException("Không tìm thấy khóa học.");
        }

        EnsureCanModify(course, currentUserId, currentUserRole);

        var maxOrder = await _unitOfWork.Lessons.GetMaxOrderIndexAsync(courseId);

        var lesson = new Lesson
        {
            CourseId = courseId,
            Title = request.Title,
            Content = request.Content,
            OrderIndex = maxOrder + 1
        };

        await _unitOfWork.Lessons.AddAsync(lesson);
        await _unitOfWork.SaveChangesAsync();

        return ToDto(lesson);
    }

    public async Task UpdateAsync(int lessonId, UpdateLessonRequest request, int currentUserId, string currentUserRole)
    {
        var lesson = await _unitOfWork.Lessons.GetWithCourseAsync(lessonId);
        if (lesson == null)
        {
            throw new NotFoundException("Không tìm thấy bài học.");
        }

        EnsureCanModify(lesson.Course, currentUserId, currentUserRole);

        lesson.Title = request.Title;
        lesson.Content = request.Content;
        lesson.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Lessons.Update(lesson);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(int lessonId, int currentUserId, string currentUserRole)
    {
        var lesson = await _unitOfWork.Lessons.GetWithCourseAsync(lessonId);
        if (lesson == null)
        {
            throw new NotFoundException("Không tìm thấy bài học.");
        }

        EnsureCanModify(lesson.Course, currentUserId, currentUserRole);

        _unitOfWork.Lessons.Remove(lesson);
        await _unitOfWork.SaveChangesAsync();
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

    private static LessonDto ToDto(Lesson l) => new()
    {
        Id = l.Id,
        Title = l.Title,
        Content = l.Content,
        OrderIndex = l.OrderIndex,
        CourseId = l.CourseId
    };
}