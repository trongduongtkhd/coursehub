namespace CourseHub.Application.DTOs.Dashboard;

public class DashboardStatsDto
{
    public int TotalUsers { get; set; }
    public int TotalAdmins { get; set; }
    public int TotalInstructors { get; set; }
    public int TotalStudents { get; set; }

    public int TotalCourses { get; set; }
    public int DraftCourses { get; set; }
    public int PublishedCourses { get; set; }
    public int ArchivedCourses { get; set; }

    public int TotalEnrollments { get; set; }
    public double AverageRatingSystemWide { get; set; }

    public List<MonthlyEnrollmentDto> EnrollmentsByMonth { get; set; } = new();
    public List<TopCourseDto> TopCourses { get; set; } = new();
}

public class MonthlyEnrollmentDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int Count { get; set; }
}

public class TopCourseDto
{
    public int CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int EnrollmentCount { get; set; }
}