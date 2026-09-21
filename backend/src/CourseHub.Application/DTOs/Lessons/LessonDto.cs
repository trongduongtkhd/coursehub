namespace CourseHub.Application.DTOs.Lessons;

public class LessonDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
    public int CourseId { get; set; }
}