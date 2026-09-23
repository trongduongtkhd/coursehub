namespace CourseHub.Application.DTOs.Courses;

public class CourseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public string Status { get; set; } = string.Empty;
    public int InstructorId { get; set; }
    public string InstructorName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
}