using DddTemplate.Api.Common;
using DddTemplate.Application.Users;
using Microsoft.AspNetCore.Mvc;

namespace DddTemplate.Api.Controllers;

/// <summary>
/// Users API 控制器
/// 演示完整的RESTful API实现，包括：
/// - 标准的CRUD操作
/// - Result模式错误处理
/// - 统一的API响应格式
/// - Swagger文档注释
/// - 结构化日志记录
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(UserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// 获取所有用户
    /// </summary>
    /// <param name="activeOnly">是否只获取活跃用户</param>
    /// <param name="ct">取消令牌</param>
    /// <returns>用户列表</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<UserDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] bool activeOnly = false, CancellationToken ct = default)
    {
        _logger.LogInformation("Getting all users (activeOnly: {ActiveOnly})", activeOnly);

        var users = activeOnly
            ? await _userService.GetActiveUsersAsync(ct)
            : await _userService.ListAsync(ct);

        _logger.LogInformation("Retrieved {Count} users", users.Count);
        return Ok(ApiResponse<IReadOnlyList<UserDto>>.SuccessResponse(users));
    }

    /// <summary>
    /// 根据ID获取用户
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <param name="ct">取消令牌</param>
    /// <returns>用户详情</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
    {
        _logger.LogInformation("Getting user by ID: {UserId}", id);

        var result = await _userService.GetByIdAsync(id, ct);

        if (result.IsFailure)
        {
            _logger.LogWarning("User {UserId} not found", id);
            return NotFound(ApiResponse<UserDto>.FailureResponse(
                new ErrorDetails(result.Error.Code, result.Error.Message, result.Error.Type.ToString())));
        }

        _logger.LogInformation("Retrieved user: {UserId}", id);
        return Ok(ApiResponse<UserDto>.SuccessResponse(result.Value));
    }

    /// <summary>
    /// 根据邮箱获取用户
    /// </summary>
    /// <param name="email">用户邮箱</param>
    /// <param name="ct">取消令牌</param>
    /// <returns>用户详情</returns>
    [HttpGet("by-email/{email}")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByEmail(string email, CancellationToken ct = default)
    {
        _logger.LogInformation("Getting user by email: {Email}", email);

        var result = await _userService.GetByEmailAsync(email, ct);

        if (result.IsFailure)
        {
            var statusCode = result.Error.Type switch
            {
                Domain.Abstractions.ErrorType.NotFound => StatusCodes.Status404NotFound,
                Domain.Abstractions.ErrorType.Validation => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status400BadRequest
            };

            _logger.LogWarning("Failed to get user by email {Email}: {Error}", email, result.Error.Message);
            return StatusCode(statusCode, ApiResponse<UserDto>.FailureResponse(
                new ErrorDetails(result.Error.Code, result.Error.Message, result.Error.Type.ToString())));
        }

        _logger.LogInformation("Retrieved user by email: {Email}", email);
        return Ok(ApiResponse<UserDto>.SuccessResponse(result.Value));
    }

    /// <summary>
    /// 创建新用户
    /// </summary>
    /// <param name="request">创建用户请求</param>
    /// <param name="ct">取消令牌</param>
    /// <returns>创建的用户</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request, CancellationToken ct = default)
    {
        _logger.LogInformation("Creating user with email: {Email}", request.Email);

        var result = await _userService.CreateAsync(request, ct);

        if (result.IsFailure)
        {
            var statusCode = result.Error.Type switch
            {
                Domain.Abstractions.ErrorType.Conflict => StatusCodes.Status409Conflict,
                Domain.Abstractions.ErrorType.Validation => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status400BadRequest
            };

            _logger.LogWarning("Failed to create user: {Error}", result.Error.Message);
            return StatusCode(statusCode, ApiResponse<UserDto>.FailureResponse(
                new ErrorDetails(result.Error.Code, result.Error.Message, result.Error.Type.ToString())));
        }

        _logger.LogInformation("User created successfully: {UserId}", result.Value.Id);
        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value.Id },
            ApiResponse<UserDto>.SuccessResponse(result.Value));
    }

    /// <summary>
    /// 更新用户
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <param name="request">更新用户请求</param>
    /// <param name="ct">取消令牌</param>
    /// <returns>更新后的用户</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest request, CancellationToken ct = default)
    {
        _logger.LogInformation("Updating user: {UserId}", id);

        var result = await _userService.UpdateAsync(id, request, ct);

        if (result.IsFailure)
        {
            var statusCode = result.Error.Type switch
            {
                Domain.Abstractions.ErrorType.NotFound => StatusCodes.Status404NotFound,
                Domain.Abstractions.ErrorType.Validation => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status400BadRequest
            };

            _logger.LogWarning("Failed to update user {UserId}: {Error}", id, result.Error.Message);
            return StatusCode(statusCode, ApiResponse<UserDto>.FailureResponse(
                new ErrorDetails(result.Error.Code, result.Error.Message, result.Error.Type.ToString())));
        }

        _logger.LogInformation("User {UserId} updated successfully", id);
        return Ok(ApiResponse<UserDto>.SuccessResponse(result.Value));
    }

    /// <summary>
    /// 停用用户
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <param name="request">停用请求</param>
    /// <param name="ct">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpPost("{id:guid}/deactivate")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Deactivate(Guid id, [FromBody] DeactivateUserRequest request, CancellationToken ct = default)
    {
        _logger.LogInformation("Deactivating user: {UserId}", id);

        var result = await _userService.DeactivateAsync(id, request, ct);

        if (result.IsFailure)
        {
            var statusCode = result.Error.Type switch
            {
                Domain.Abstractions.ErrorType.NotFound => StatusCodes.Status404NotFound,
                Domain.Abstractions.ErrorType.Conflict => StatusCodes.Status409Conflict,
                Domain.Abstractions.ErrorType.Validation => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status400BadRequest
            };

            _logger.LogWarning("Failed to deactivate user {UserId}: {Error}", id, result.Error.Message);
            return StatusCode(statusCode, ApiResponse.FailureResponse(
                new ErrorDetails(result.Error.Code, result.Error.Message, result.Error.Type.ToString())));
        }

        _logger.LogInformation("User {UserId} deactivated successfully", id);
        return Ok(ApiResponse.SuccessResponse("User deactivated successfully"));
    }

    /// <summary>
    /// 激活用户
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <param name="ct">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpPost("{id:guid}/activate")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Activate(Guid id, CancellationToken ct = default)
    {
        _logger.LogInformation("Activating user: {UserId}", id);

        var result = await _userService.ActivateAsync(id, ct);

        if (result.IsFailure)
        {
            var statusCode = result.Error.Type switch
            {
                Domain.Abstractions.ErrorType.NotFound => StatusCodes.Status404NotFound,
                Domain.Abstractions.ErrorType.Conflict => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status400BadRequest
            };

            _logger.LogWarning("Failed to activate user {UserId}: {Error}", id, result.Error.Message);
            return StatusCode(statusCode, ApiResponse.FailureResponse(
                new ErrorDetails(result.Error.Code, result.Error.Message, result.Error.Type.ToString())));
        }

        _logger.LogInformation("User {UserId} activated successfully", id);
        return Ok(ApiResponse.SuccessResponse("User activated successfully"));
    }

    /// <summary>
    /// 删除用户
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <param name="ct">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        _logger.LogInformation("Deleting user: {UserId}", id);

        var result = await _userService.DeleteAsync(id, ct);

        if (result.IsFailure)
        {
            _logger.LogWarning("Failed to delete user {UserId}: {Error}", id, result.Error.Message);
            return NotFound(ApiResponse.FailureResponse(
                new ErrorDetails(result.Error.Code, result.Error.Message, result.Error.Type.ToString())));
        }

        _logger.LogInformation("User {UserId} deleted successfully", id);
        return Ok(ApiResponse.SuccessResponse("User deleted successfully"));
    }
}
