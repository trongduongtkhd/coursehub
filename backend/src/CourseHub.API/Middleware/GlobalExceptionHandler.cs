using CourseHub.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CourseHub.API.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment env)
    {
        _logger = logger;
        _env = env;  
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, title) = MapException(exception);

        _logger.LogError(exception, "Lỗi khi xử lý {Method} {Path}", httpContext.Request.Method, httpContext.Request.Path);

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = exception.Message,
            Instance = httpContext.Request.Path
        };
        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

        if (statusCode == StatusCodes.Status500InternalServerError && !_env.IsDevelopment())
        {
            problemDetails.Detail = "Đã xảy ra lỗi không mong muốn. Vui lòng thử lại, kèm mã traceId nếu cần báo lỗi.";
        }

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private static (int StatusCode, string Title) MapException(Exception exception) => exception switch
    {
        NotFoundException => (StatusCodes.Status404NotFound, "Không tìm thấy"),
        ForbiddenException => (StatusCodes.Status403Forbidden, "Không có quyền"),
        ConflictException => (StatusCodes.Status409Conflict, "Xung đột dữ liệu"),
        BadRequestException => (StatusCodes.Status400BadRequest, "Yêu cầu không hợp lệ"),
        UnauthorizedException => (StatusCodes.Status401Unauthorized, "Không xác thực được"),
        _ => (StatusCodes.Status500InternalServerError, "Lỗi hệ thống")
    };
}