using CourseHub.Application.Exceptions;
using CourseHub.Application.Interfaces.Repositories;
using CourseHub.Application.Services;
using CourseHub.Domain.Entities;
using CourseHub.Domain.Enums;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace CourseHub.UnitTests;

public class EnrollmentServiceTests
{
    [Fact]
    public async Task EnrollAsync_WhenCourseIsDraft_ThrowsBadRequest()
    {
        // 1. Chuẩn bị đồ giả
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var draftCourse = new Course
        {
            Id = 1,
            Status = CourseStatus.Draft,
            InstructorId = 99,
            Instructor = new User { FullName = "Giảng viên A" }
        };
        unitOfWork.Courses.GetWithDetailsAsync(1).Returns(draftCourse);

        var service = new EnrollmentService(unitOfWork, NullLogger<EnrollmentService>.Instance);

        // 2. Gọi code thật và kiểm tra đáp án
        await Assert.ThrowsAsync<BadRequestException>(() => service.EnrollAsync(1, 5));
        await unitOfWork.DidNotReceive().SaveChangesAsync();
    }
    [Fact]
public async Task EnrollAsync_WhenValid_SavesEnrollment()
{
    // 1. Chuẩn bị đồ giả
    var unitOfWork = Substitute.For<IUnitOfWork>();

    var publishedCourse = new Course
    {
        Id = 1,
        Status = CourseStatus.Published,
        InstructorId = 99,
        Instructor = new User { FullName = "Giảng viên A" }
    };
    unitOfWork.Courses.GetWithDetailsAsync(1).Returns(publishedCourse);
    unitOfWork.Enrollments.ExistsAsync(5, 1).Returns(false);

    var service = new EnrollmentService(unitOfWork, NullLogger<EnrollmentService>.Instance);

    // 2. Gọi code thật
    var result = await service.EnrollAsync(1, 5);

    // 3. Kiểm tra
    Assert.Equal(1, result.CourseId);

    await unitOfWork.Enrollments.Received(1).AddAsync(
        Arg.Is<Enrollment>(e => e.UserId == 5 && e.CourseId == 1));

    await unitOfWork.Received(1).SaveChangesAsync();
}
}