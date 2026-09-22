namespace CourseHub.Application.DTOs.Courses;

public class CourseQueryParameters
{
    public string? Search { get; set; }
    public string? Status { get; set; }
    public int? InstructorId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SortBy { get; set; } = "createdAt";
    public string SortDir { get; set; } = "desc";
}