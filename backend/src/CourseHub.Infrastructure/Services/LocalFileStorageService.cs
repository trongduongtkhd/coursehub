using CourseHub.Application.Exceptions;
using CourseHub.Application.Interfaces.Services;
using Microsoft.AspNetCore.Hosting;

namespace CourseHub.Infrastructure.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _env;

    private static readonly HashSet<string> AllowedExtensions = new() { ".jpg", ".jpeg", ".png", ".webp" };

    private static readonly Dictionary<string, byte[]> MagicNumbers = new()
    {
        [".jpg"] = new byte[] { 0xFF, 0xD8, 0xFF },
        [".jpeg"] = new byte[] { 0xFF, 0xD8, 0xFF },
        [".png"] = new byte[] { 0x89, 0x50, 0x4E, 0x47 }
    };

    public LocalFileStorageService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<string> SaveCourseThumbnailAsync(Stream fileStream, string fileExtension)
    {
        var ext = fileExtension.ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext))
        {
            throw new BadRequestException("Chỉ chấp nhận file .jpg, .jpeg, .png hoặc .webp.");
        }

        var header = new byte[12];
        var bytesRead = await fileStream.ReadAsync(header, 0, header.Length);
        fileStream.Position = 0;

        if (!IsValidImageSignature(ext, header, bytesRead))
        {
            throw new BadRequestException("Nội dung file không đúng định dạng ảnh đã khai báo.");
        }

        var fileName = $"{Guid.NewGuid()}{ext}";
        var uploadsFolder = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads");
        Directory.CreateDirectory(uploadsFolder);

        var filePath = Path.Combine(uploadsFolder, fileName);
        using (var output = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(output);
        }

        return $"/uploads/{fileName}";
    }

    private static bool IsValidImageSignature(string ext, byte[] header, int bytesRead)
    {
        if (ext == ".webp")
        {
            return bytesRead >= 12
                && header[0] == 0x52 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x46
                && header[8] == 0x57 && header[9] == 0x45 && header[10] == 0x42 && header[11] == 0x50;
        }

        if (!MagicNumbers.TryGetValue(ext, out var signature) || bytesRead < signature.Length)
        {
            return false;
        }

        for (int i = 0; i < signature.Length; i++)
        {
            if (header[i] != signature[i]) return false;
        }
        return true;
    }
}