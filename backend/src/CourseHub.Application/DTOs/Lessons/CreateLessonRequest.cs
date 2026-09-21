namespace CourseHub.Application.DTOs.Lessons;

public class CreateLessonRequest
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}