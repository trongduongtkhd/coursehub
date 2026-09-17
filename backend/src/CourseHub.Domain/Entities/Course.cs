using CourseHub.Domain.Common;
using CourseHub.Domain.Enums;

namespace CourseHub.Domain.Entities;

public class Course : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public CourseStatus Status { get; set; } = CourseStatus.Draft;
    public bool IsDeleted { get; set; } = false;

    public int InstructorId { get; set; }
    public User Instructor { get; set; } = null!;

    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}