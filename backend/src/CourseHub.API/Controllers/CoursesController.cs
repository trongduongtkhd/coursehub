using System.Security.Claims;
using CourseHub.Application.DTOs.Courses;
using CourseHub.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CourseHub.Application.DTOs.Enrollments;
using System.Xml.Serialization;
using CourseHub.Application.DTOs.Common;
namespace CourseHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;
private readonly IProgressService _progressService;
    public CoursesController(ICourseService courseService, IProgressService progressService)
    {
        _courseService = courseService;
        _progressService = progressService;
    }

  [HttpGet]
public async Task<ActionResult<PagedResult<CourseDto>>> GetAll([FromQuery] CourseQueryParameters query)
    => Ok(await _courseService.GetAllAsync(query));

[HttpGet("{id}")]
public async Task<ActionResult<CourseDto>> GetById(int id)
    => Ok(await _courseService.GetByIdAsync(id));

[Authorize(Roles = "Admin,Instructor")]
[HttpPost]
public async Task<ActionResult<CourseDto>> Create(CreateCourseRequest request)
{
    var course = await _courseService.CreateAsync(request, GetCurrentUserId());
    return CreatedAtAction(nameof(GetById), new { id = course.Id }, course);
}

[Authorize(Roles = "Admin,Instructor")]
[HttpPut("{id}")]
public async Task<IActionResult> Update(int id, UpdateCourseRequest request)
{
    await _courseService.UpdateAsync(id, request, GetCurrentUserId(), GetCurrentUserRole());
    return NoContent();
}

[Authorize(Roles = "Admin,Instructor")]
[HttpDelete("{id}")]
public async Task<IActionResult> Delete(int id)
{
    await _courseService.DeleteAsync(id, GetCurrentUserId(), GetCurrentUserRole());
    return NoContent();
}

[Authorize]
[HttpGet("{id}/my-progress")]
public async Task<ActionResult<List<LessonProgressDto>>> GetMyProgress(int id)
    => Ok(await _progressService.GetCourseProgressAsync(id, GetCurrentUserId()));

[Authorize(Roles = "Admin,Instructor")]
[HttpPost("{id}/thumbnail")]
[RequestSizeLimit(2 * 1024 * 1024)]
public async Task<IActionResult> UploadThumbnail(int id, IFormFile file)
{
    if (file == null || file.Length == 0)
    {
        return BadRequest(new { message = "Vui lòng chọn một file ảnh." });
    }

    var extension = Path.GetExtension(file.FileName);
    using var stream = file.OpenReadStream();
    var url = await _courseService.UpdateThumbnailAsync(id, stream, extension, GetCurrentUserId(), GetCurrentUserRole());
    return Ok(new { thumbnailUrl = url });
}

private int GetCurrentUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
private string GetCurrentUserRole() => User.FindFirstValue(ClaimTypes.Role)!;
}