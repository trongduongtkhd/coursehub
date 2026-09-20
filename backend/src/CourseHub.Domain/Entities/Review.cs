using CourseHub.Domain.Common;

namespace CourseHub.Domain.Entities;

public class Review : BaseEntity
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;

    public int Rating { get; set; }
    public string? Comment { get; set; }
}