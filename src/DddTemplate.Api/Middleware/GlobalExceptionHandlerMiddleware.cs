using System.Net;
using System.Text.Json;
using DddTemplate.Api.Common;
using DddTemplate.Domain.Abstractions;

namespace DddTemplate.Api.Middleware;

/// <summary>
/// 全局异常处理中间件
/// 捕获所有未处理的异常并返回统一的错误响应
/// </summary>
public sealed class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, errorDetails) = exception switch
        {
            ValidationException validationEx => (
                HttpStatusCode.BadRequest,
                new ErrorDetails(
                    validationEx.Error.Code,
                    validationEx.Error.Message,
                    "Validation",
                    _environment.IsDevelopment() ? exception.StackTrace : null
                )
            ),
            NotFoundException notFoundEx => (
                HttpStatusCode.NotFound,
                new ErrorDetails(
                    notFoundEx.Error.Code,
                    notFoundEx.Error.Message,
                    "NotFound",
                    _environment.IsDevelopment() ? exception.StackTrace : null
                )
            ),
            ConflictException conflictEx => (
                HttpStatusCode.Conflict,
                new ErrorDetails(
                    conflictEx.Error.Code,
                    conflictEx.Error.Message,
                    "Conflict",
                    _environment.IsDevelopment() ? exception.StackTrace : null
                )
            ),
            BusinessRuleException businessEx => (
                HttpStatusCode.UnprocessableEntity,
                new ErrorDetails(
                    businessEx.Error.Code,
                    businessEx.Error.Message,
                    "BusinessRule",
                    _environment.IsDevelopment() ? exception.StackTrace : null
                )
            ),
            DomainException domainEx => (
                HttpStatusCode.BadRequest,
                new ErrorDetails(
                    domainEx.Error.Code,
                    domainEx.Error.Message,
                    "Domain",
                    _environment.IsDevelopment() ? exception.StackTrace : null
                )
            ),
            ArgumentException argEx => (
                HttpStatusCode.BadRequest,
                new ErrorDetails(
                    "Argument.Invalid",
                    argEx.Message,
                    "Validation",
                    _environment.IsDevelopment() ? exception.StackTrace : null
                )
            ),
            _ => (
                HttpStatusCode.InternalServerError,
                new ErrorDetails(
                    "Server.InternalError",
                    _environment.IsDevelopment()
                        ? exception.Message
                        : "An internal server error occurred. Please try again later.",
                    "Internal",
                    _environment.IsDevelopment() ? exception.StackTrace : null
                )
            )
        };

        context.Response.StatusCode = (int)statusCode;

        var response = ApiResponse.FailureResponse(errorDetails);

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = _environment.IsDevelopment()
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}

/// <summary>
/// 全局异常处理中间件扩展方法
/// </summary>
public static class GlobalExceptionHandlerMiddlewareExtensions
{
    /// <summary>
    /// 使用全局异常处理中间件
    /// </summary>
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }
}
