using CourseHub.Domain.Common;
using CourseHub.Domain.Enums;

namespace CourseHub.Domain.Entities;

public class User : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public Role Role { get; set; }
    public string? AvatarUrl { get; set; }

    public ICollection<Course> CoursesTaught { get; set; } = new List<Course>();
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<LessonProgress> LessonProgresses { get; set; } = new List<LessonProgress>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}   