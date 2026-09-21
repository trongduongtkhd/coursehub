namespace CourseHub.Application.DTOs.Enrollments;

public class LessonProgressDto
{
    public int LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
    public bool IsCompleted { get; set; }
}