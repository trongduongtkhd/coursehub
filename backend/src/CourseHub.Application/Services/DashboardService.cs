using CourseHub.Application.DTOs.Dashboard;
using CourseHub.Application.Interfaces;
using CourseHub.Application.Interfaces.Services;
using CourseHub.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CourseHub.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IAppDbContext _context;
    private readonly ICacheService _cache;
    public DashboardService(IAppDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<DashboardStatsDto> GetStatsAsync()
    {

    const string cacheKey = "dashboard:stats";
    var cached = _cache.Get<DashboardStatsDto>(cacheKey);
    if (cached != null)
    {
        return cached;
    }

        var usersByRole = await _context.Users
            .GroupBy(u => u.Role)
            .Select(g => new { Role = g.Key, Count = g.Count() })
            .ToListAsync();

        var coursesByStatus = await _context.Courses
            .GroupBy(c => c.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync();

        var totalEnrollments = await _context.Enrollments.CountAsync();

        var reviewCount = await _context.Reviews.CountAsync();
        var avgRating = reviewCount == 0 ? 0 : Math.Round(await _context.Reviews.AverageAsync(r => r.Rating), 1);

        var sixMonthsAgo = DateTime.UtcNow.AddMonths(-5);
        var enrollmentsByMonth = await _context.Enrollments
            .Where(e => e.EnrolledAt >= sixMonthsAgo)
            .GroupBy(e => new { e.EnrolledAt.Year, e.EnrolledAt.Month })
            .Select(g => new MonthlyEnrollmentDto
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Count = g.Count()
            })
            .OrderBy(x => x.Year).ThenBy(x => x.Month)
            .ToListAsync();

        var topCourses = await _context.Courses
            .Select(c => new TopCourseDto
            {
                CourseId = c.Id,
                Title = c.Title,
                EnrollmentCount = c.Enrollments.Count()
            })
            .OrderByDescending(c => c.EnrollmentCount)
            .Take(5)
            .ToListAsync();

        var result =  new DashboardStatsDto
        {
            TotalUsers = usersByRole.Sum(x => x.Count),
            TotalAdmins = usersByRole.FirstOrDefault(x => x.Role == Role.Admin)?.Count ?? 0,
            TotalInstructors = usersByRole.FirstOrDefault(x => x.Role == Role.Instructor)?.Count ?? 0,
            TotalStudents = usersByRole.FirstOrDefault(x => x.Role == Role.Student)?.Count ?? 0,

            TotalCourses = coursesByStatus.Sum(x => x.Count),
            DraftCourses = coursesByStatus.FirstOrDefault(x => x.Status == CourseStatus.Draft)?.Count ?? 0,
            PublishedCourses = coursesByStatus.FirstOrDefault(x => x.Status == CourseStatus.Published)?.Count ?? 0,
            ArchivedCourses = coursesByStatus.FirstOrDefault(x => x.Status == CourseStatus.Archived)?.Count ?? 0,

            TotalEnrollments = totalEnrollments,
            AverageRatingSystemWide = avgRating,

            EnrollmentsByMonth = enrollmentsByMonth,
            TopCourses = topCourses
        };
          _cache.Set(cacheKey, result, TimeSpan.FromMinutes(2));
          return result;
        
    }
}