using CourseHub.Domain.Common;

namespace CourseHub.Domain.Entities;

public class LessonProgress : BaseEntity
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int LessonId { get; set; }
    public Lesson Lesson { get; set; } = null!;

    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
}