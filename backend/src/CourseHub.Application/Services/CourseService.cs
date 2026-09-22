using CourseHub.Application.DTOs.Courses;
using CourseHub.Application.Exceptions;
using CourseHub.Application.Interfaces;
using CourseHub.Application.Interfaces.Services;
using CourseHub.Domain.Entities;
using CourseHub.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using CourseHub.Application.DTOs.Common;
namespace CourseHub.Application.Services;

public class CourseService : ICourseService
{
    private readonly IAppDbContext _context;
    private readonly ICacheService _cache;
    private readonly IFileStorageService _fileStorageService;

    public CourseService(IAppDbContext context, IFileStorageService fileStorageService,ICacheService cache)
    {
        _context = context;
        _fileStorageService = fileStorageService;
        _cache = cache;
    }

   public async Task<PagedResult<CourseDto>> GetAllAsync(CourseQueryParameters query)
{
    var pageSize = Math.Clamp(query.PageSize, 1, 100);
    var page = Math.Max(query.Page, 1);

    var courses = _context.Courses.Include(c => c.Instructor).Include(c => c.Reviews).AsQueryable();

    if (!string.IsNullOrWhiteSpace(query.Search))
    {
        courses = courses.Where(c => c.Title.Contains(query.Search) || c.Description.Contains(query.Search));
    }

    if (!string.IsNullOrWhiteSpace(query.Status) && Enum.TryParse<CourseStatus>(query.Status, true, out var statusEnum))
    {
        courses = courses.Where(c => c.Status == statusEnum);
    }

    if (query.InstructorId.HasValue)
    {
        courses = courses.Where(c => c.InstructorId == query.InstructorId.Value);
    }

    courses = query.SortBy?.ToLower() switch
    {
        "title" => query.SortDir == "asc" ? courses.OrderBy(c => c.Title) : courses.OrderByDescending(c => c.Title),
        _ => query.SortDir == "asc" ? courses.OrderBy(c => c.CreatedAt) : courses.OrderByDescending(c => c.CreatedAt)
    };

    var totalCount = await courses.CountAsync();

    var items = await courses
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(c => ToDto(c))
        .ToListAsync();

    return new PagedResult<CourseDto>
    {
        Items = items,
        Page = page,
        PageSize = pageSize,
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

    var course = await _context.Courses
        .Include(c => c.Instructor)
        .Include(c => c.Reviews)
        .FirstOrDefaultAsync(c => c.Id == id);

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
       _cache.Remove($"course:{id}"); 
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
        _cache.Remove($"course:{id}");
      
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

    public async Task<string> UpdateThumbnailAsync(int courseId, Stream fileStream, string fileExtension, int currentUserId, string currentUserRole)
{
    var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);
    if (course == null)
    {
        throw new NotFoundException("Không tìm thấy khóa học.");
    }

    EnsureCanModify(course, currentUserId, currentUserRole);

    var url = await _fileStorageService.SaveCourseThumbnailAsync(fileStream, fileExtension);

    course.ThumbnailUrl = url;
    course.UpdatedAt = DateTime.UtcNow;
    await _context.SaveChangesAsync();
    _cache.Remove($"course:{courseId}");
    return url;
}
}