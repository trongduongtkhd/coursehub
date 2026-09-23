using CourseHub.Application.DTOs.Courses;
using CourseHub.Domain.Entities;

namespace CourseHub.Application.Interfaces.Repositories;

public interface ICourseRepository : IRepository<Course>
{
    Task<Course?> GetWithDetailsAsync(int id);
    Task<(List<Course> Items, int TotalCount)> GetPagedAsync(CourseQueryParameters query);
}   