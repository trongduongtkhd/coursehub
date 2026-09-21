using System.Security.Claims;
using CourseHub.Application.DTOs.Courses;
using CourseHub.Application.Exceptions;
using CourseHub.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CourseHub.Application.DTOs.Enrollments;
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
    public async Task<ActionResult<List<CourseDto>>> GetAll()
        => Ok(await _courseService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<CourseDto>> GetById(int id)
    {
        try
        {
            return Ok(await _courseService.GetByIdAsync(id));
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

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
        try
        {
            await _courseService.UpdateAsync(id, request, GetCurrentUserId(), GetCurrentUserRole());
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ForbiddenException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
    }

    [Authorize(Roles = "Admin,Instructor")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _courseService.DeleteAsync(id, GetCurrentUserId(), GetCurrentUserRole());
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ForbiddenException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
    }

    [Authorize]
[HttpGet("{id}/my-progress")]
public async Task<ActionResult<List<LessonProgressDto>>> GetMyProgress(int id)
{
    try
    {
        return Ok(await _progressService.GetCourseProgressAsync(id, GetCurrentUserId()));
    }
    catch (ForbiddenException ex) { return StatusCode(403, new { message = ex.Message }); }
}

    private int GetCurrentUserId()
        => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private string GetCurrentUserRole()
        => User.FindFirstValue(ClaimTypes.Role)!;
}