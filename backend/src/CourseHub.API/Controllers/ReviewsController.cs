using System.Security.Claims;
using CourseHub.Application.DTOs.Reviews;
using CourseHub.Application.Exceptions;
using CourseHub.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseHub.API.Controllers;

[ApiController]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet("api/courses/{courseId}/reviews")]
    public async Task<ActionResult<List<ReviewDto>>> GetByCourse(int courseId)
        => Ok(await _reviewService.GetByCourseAsync(courseId));

    [HttpGet("api/courses/{courseId}/reviews/summary")]
    public async Task<ActionResult<CourseRatingSummaryDto>> GetSummary(int courseId)
        => Ok(await _reviewService.GetRatingSummaryAsync(courseId));

    [Authorize]
    [HttpPost("api/courses/{courseId}/reviews")]
    public async Task<ActionResult<ReviewDto>> Upsert(int courseId, CreateReviewRequest request)
    {
            var review = await _reviewService.UpsertAsync(courseId, request, GetCurrentUserId());
            return Ok(review);
      
    }

    private int GetCurrentUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}