using CourseHub.Application.Interfaces.Repositories;
using CourseHub.Infrastructure.Persistence;

namespace CourseHub.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private ICourseRepository? _courses;
    private ILessonProgressRepository? _lessonProgresses;
    private ILessonRepository? _lessons;
    private IEnrollmentRepository? _enrollments;
    private IUserRepository? _users;
    private IReviewRepository? _reviews;
    private IRefreshTokenRepository? _refreshTokens;
    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public ICourseRepository Courses => _courses ??= new CourseRepository(_context);
    public ILessonRepository Lessons => _lessons ??= new LessonRepository(_context);
    public IEnrollmentRepository Enrollments => _enrollments ??= new EnrollmentRepository(_context);
    public ILessonProgressRepository LessonProgresses => _lessonProgresses ??= new LessonProgressRepository(_context);
    public IUserRepository Users => _users ??= new UserRepository(_context);
    public IReviewRepository Reviews => _reviews ??= new ReviewRepository(_context);
    public IRefreshTokenRepository RefreshTokens => _refreshTokens ??= new RefreshTokenRepository(_context);
    public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();
}