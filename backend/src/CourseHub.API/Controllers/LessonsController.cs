using System.Security.Claims;
using CourseHub.Application.DTOs.Lessons;
using CourseHub.Application.Exceptions;
using CourseHub.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseHub.API.Controllers;

[ApiController]
public class LessonsController : ControllerBase
{
    private readonly ILessonService _lessonService;
    private readonly IProgressService _progressService;

    public LessonsController(ILessonService lessonService, IProgressService progressService)
    {
        _lessonService = lessonService;
        _progressService = progressService;
    }

    [HttpGet("api/courses/{courseId}/lessons")]
    public async Task<ActionResult<List<LessonDto>>> GetByCourse(int courseId)
        => Ok(await _lessonService.GetByCourseAsync(courseId));

    [Authorize(Roles = "Admin,Instructor")]
    [HttpPost("api/courses/{courseId}/lessons")]
    public async Task<ActionResult<LessonDto>> Create(int courseId, CreateLessonRequest request)
    {
       
            var lesson = await _lessonService.CreateAsync(courseId, request, GetCurrentUserId(), GetCurrentUserRole());
            return StatusCode(201, lesson);
       
    }

    [Authorize(Roles = "Admin,Instructor")]
    [HttpPut("api/lessons/{id}")]
    public async Task<IActionResult> Update(int id, UpdateLessonRequest request)
    {
      
            await _lessonService.UpdateAsync(id, request, GetCurrentUserId(), GetCurrentUserRole());
            return NoContent();
  
    }

    [Authorize(Roles = "Admin,Instructor")]
    [HttpDelete("api/lessons/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
       
            await _lessonService.DeleteAsync(id, GetCurrentUserId(), GetCurrentUserRole());
            return NoContent();
   }

[Authorize]
[HttpPost("api/lessons/{id}/complete")]
public async Task<IActionResult> MarkComplete(int id)
{
        await _progressService.MarkCompleteAsync(id, GetCurrentUserId());
        return NoContent();
}

[Authorize]
[HttpDelete("api/lessons/{id}/complete")]
public async Task<IActionResult> UnmarkComplete(int id)
{
    await _progressService.UnmarkCompleteAsync(id, GetCurrentUserId());
    return NoContent();
}

    private int GetCurrentUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private string GetCurrentUserRole() => User.FindFirstValue(ClaimTypes.Role)!;
}