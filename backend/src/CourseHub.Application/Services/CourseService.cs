using CourseHub.Application.DTOs.Courses;
using CourseHub.Application.Exceptions;
using CourseHub.Application.Interfaces;
using CourseHub.Application.Interfaces.Services;
using CourseHub.Domain.Entities;
using CourseHub.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CourseHub.Application.Services;

public class CourseService : ICourseService
{
    private readonly IAppDbContext _context;

    public CourseService(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<CourseDto>> GetAllAsync()
    {
        return await _context.Courses
            .Include(c => c.Instructor)
            .Select(c => ToDto(c))
            .ToListAsync();
    }

    public async Task<CourseDto> GetByIdAsync(int id)
    {
        var course = await _context.Courses
            .Include(c => c.Instructor)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null)
        {
            throw new NotFoundException("Không tìm thấy khóa học.");
        }

        return ToDto(course);
    }

    public async Task<CourseDto> CreateAsync(CreateCourseRequest request, int instructorId)
    {
        var course = new Course
        {
            Title = request.Title,
            Description = request.Description,
            InstructorId = instructorId,
            Status = CourseStatus.Draft
        };

        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(course.Id);
    }

    public async Task UpdateAsync(int id, UpdateCourseRequest request, int currentUserId, string currentUserRole)
    {
        var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
        if (course == null)
        {
            throw new NotFoundException("Không tìm thấy khóa học.");
        }

        EnsureCanModify(course, currentUserId, currentUserRole);

        course.Title = request.Title;
        course.Description = request.Description;
        course.Status = Enum.Parse<CourseStatus>(request.Status);
        course.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id, int currentUserId, string currentUserRole)
    {
        var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
        if (course == null)
        {
            throw new NotFoundException("Không tìm thấy khóa học.");
        }

        EnsureCanModify(course, currentUserId, currentUserRole);

        course.IsDeleted = true;
        await _context.SaveChangesAsync();
    }

    private static void EnsureCanModify(Course course, int currentUserId, string currentUserRole)
    {
        var isOwner = course.InstructorId == currentUserId;
        var isAdmin = currentUserRole == Role.Admin.ToString();

        if (!isOwner && !isAdmin)
        {
            throw new ForbiddenException("Bạn không có quyền chỉnh sửa khóa học này.");
        }
    }

    private static CourseDto ToDto(Course c) => new()
    {
        Id = c.Id,
        Title = c.Title,
        Description = c.Description,
        ThumbnailUrl = c.ThumbnailUrl,
        Status = c.Status.ToString(),
        InstructorId = c.InstructorId,
        InstructorName = c.Instructor.FullName,
        CreatedAt = c.CreatedAt
    };
}