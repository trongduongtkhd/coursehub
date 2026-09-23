using CourseHub.Application.DTOs.Common;
using CourseHub.Application.DTOs.Courses;
using CourseHub.Application.Exceptions;
using CourseHub.Application.Interfaces.Repositories;
using CourseHub.Application.Interfaces.Services;
using CourseHub.Domain.Entities;
using CourseHub.Domain.Enums;

namespace CourseHub.Application.Services;

public class CourseService : ICourseService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICacheService _cache;

    public CourseService(IUnitOfWork unitOfWork, IFileStorageService fileStorageService, ICacheService cache)
    {
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
        _cache = cache;
    }

    public async Task<PagedResult<CourseDto>> GetAllAsync(CourseQueryParameters query)
    {
        var (courses, totalCount) = await _unitOfWork.Courses.GetPagedAsync(query);

        return new PagedResult<CourseDto>
        {
            Items = courses.Select(ToDto).ToList(),
            Page = Math.Max(query.Page, 1),
            PageSize = Math.Clamp(query.PageSize, 1, 100),
            TotalCount = totalCount
        };
    }

    public async Task<CourseDto> GetByIdAsync(int id)
    {
        var cacheKey = $"course:{id}";
        var cached = _cache.Get<CourseDto>(cacheKey);
        if (cached != null)
        {
            return cached;
        }

        var course = await _unitOfWork.Courses.GetWithDetailsAsync(id);
        if (course == null)
        {
            throw new NotFoundException("Không tìm thấy khóa học.");
        }

        var dto = ToDto(course);
        _cache.Set(cacheKey, dto, TimeSpan.FromMinutes(5));

        return dto;
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

        await _unitOfWork.Courses.AddAsync(course);
        await _unitOfWork.SaveChangesAsync();

        return await GetByIdAsync(course.Id);
    }

    public async Task UpdateAsync(int id, UpdateCourseRequest request, int currentUserId, string currentUserRole)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(id);
        if (course == null)
        {
            throw new NotFoundException("Không tìm thấy khóa học.");
        }

        EnsureCanModify(course, currentUserId, currentUserRole);

        course.Title = request.Title;
        course.Description = request.Description;
        course.Status = Enum.Parse<CourseStatus>(request.Status);
        course.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Courses.Update(course);
        await _unitOfWork.SaveChangesAsync();

        _cache.Remove($"course:{id}");
    }

    public async Task DeleteAsync(int id, int currentUserId, string currentUserRole)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(id);
        if (course == null)
        {
            throw new NotFoundException("Không tìm thấy khóa học.");
        }

        EnsureCanModify(course, currentUserId, currentUserRole);

        course.IsDeleted = true;
        _unitOfWork.Courses.Update(course);
        await _unitOfWork.SaveChangesAsync();

        _cache.Remove($"course:{id}");
    }

    public async Task<string> UpdateThumbnailAsync(int courseId, Stream fileStream, string fileExtension, int currentUserId, string currentUserRole)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(courseId);
        if (course == null)
        {
            throw new NotFoundException("Không tìm thấy khóa học.");
        }

        EnsureCanModify(course, currentUserId, currentUserRole);

        var url = await _fileStorageService.SaveCourseThumbnailAsync(fileStream, fileExtension);

        course.ThumbnailUrl = url;
        course.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Courses.Update(course);
        await _unitOfWork.SaveChangesAsync();

        _cache.Remove($"course:{courseId}");

        return url;
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
        CreatedAt = c.CreatedAt,
        AverageRating = c.Reviews.Any() ? Math.Round(c.Reviews.Average(r => r.Rating), 1) : 0,
        ReviewCount = c.Reviews.Count
    };
}