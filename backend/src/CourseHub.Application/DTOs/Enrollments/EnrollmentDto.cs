namespace CourseHub.Application.DTOs.Enrollments;

public class EnrollmentDto
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public string? CourseThumbnailUrl { get; set; }
    public string InstructorName { get; set; } = string.Empty;
    public DateTime EnrolledAt { get; set; }
}