using System.Security.Claims;
using CourseHub.Application.DTOs.Enrollments;
using CourseHub.Application.Exceptions;
using CourseHub.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseHub.API.Controllers;

[ApiController]
[Authorize]
public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _enrollmentService;

    public EnrollmentsController(IEnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
    }

    [HttpPost("api/courses/{courseId}/enroll")]
    public async Task<ActionResult<EnrollmentDto>> Enroll(int courseId)
    {
        try
        {
            var enrollment = await _enrollmentService.EnrollAsync(courseId, GetCurrentUserId());
            return StatusCode(201, enrollment);
        }
        catch (NotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (BadRequestException ex) { return BadRequest(new { message = ex.Message }); }
        catch (ConflictException ex) { return Conflict(new { message = ex.Message }); }
    }

    [HttpGet("api/enrollments/my")]
    public async Task<ActionResult<List<EnrollmentDto>>> GetMy()
        => Ok(await _enrollmentService.GetMyEnrollmentsAsync(GetCurrentUserId()));

    private int GetCurrentUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}