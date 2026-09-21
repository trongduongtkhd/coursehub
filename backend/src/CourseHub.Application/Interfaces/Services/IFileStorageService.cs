namespace CourseHub.Application.Interfaces.Services;

public interface IFileStorageService
{
    Task<string> SaveCourseThumbnailAsync(Stream fileStream, string fileExtension);
}