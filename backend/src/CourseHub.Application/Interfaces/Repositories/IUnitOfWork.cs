namespace CourseHub.Application.Interfaces.Repositories;

public interface IUnitOfWork
{
    ICourseRepository Courses { get; }
    ILessonRepository Lessons { get; }
    IEnrollmentRepository Enrollments { get; }
    ILessonProgressRepository LessonProgresses { get;}
    IUserRepository Users { get; }
    IReviewRepository Reviews { get; }
    IRefreshTokenRepository RefreshTokens { get; }
    Task<int> SaveChangesAsync();
}