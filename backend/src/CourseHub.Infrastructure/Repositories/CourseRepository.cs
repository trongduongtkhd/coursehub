using CourseHub.Application.DTOs.Courses;
using CourseHub.Application.Interfaces.Repositories;
using CourseHub.Domain.Entities;
using CourseHub.Domain.Enums;
using CourseHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CourseHub.Infrastructure.Repositories;

public class CourseRepository : Repository<Course>, ICourseRepository
{
    public CourseRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Course?> GetWithDetailsAsync(int id)
    {
        return await DbSet
            .Include(c => c.Instructor)
            .Include(c => c.Reviews)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<(List<Course> Items, int TotalCount)> GetPagedAsync(CourseQueryParameters query)
    {
        var pageSize = Math.Clamp(query.PageSize, 1, 100);
        var page = Math.Max(query.Page, 1);

        var courses = DbSet.Include(c => c.Instructor).Include(c => c.Reviews).AsQueryable();

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

        var items = await courses.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return (items, totalCount);
    }
}