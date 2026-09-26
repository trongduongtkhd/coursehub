using CourseHub.Application.DTOs.Enrollments;

namespace CourseHub.UnitTests;

public class ProgressPercentTests
{
    [Fact]
    public void ProgressPercent_1of2Lessons_Returns50()
    {
        var dto = new EnrollmentDto { TotalLessons = 2, CompletedLessons = 1 };

        Assert.Equal(50, dto.ProgressPercent);
    }
}