using DddTemplate.Domain.Abstractions;
using DddTemplate.Domain.Users;
using DddTemplate.Domain.Users.ValueObjects;
using Microsoft.Extensions.Logging;

namespace DddTemplate.Application.Users;

/// <summary>
/// User 应用服务
/// 演示完整的应用服务实现，包括：
/// - Result模式错误处理
/// - 结构化日志记录
/// - 业务逻辑编排
/// - DTO映射
/// </summary>
public sealed class UserService
{
    private readonly IUserRepository _repository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository repository, ILogger<UserService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// 创建用户
    /// </summary>
    public async Task<Result<UserDto>> CreateAsync(CreateUserRequest request, CancellationToken ct = default)
    {
        _logger.LogInformation("Creating user with email: {Email}", request.Email);

        // 创建Email值对象
        var emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure)
        {
            _logger.LogWarning("Invalid email format: {Email}", request.Email);
            return Result.Failure<UserDto>(emailResult.Error);
        }

        // 检查邮箱是否已存在
        var emailExists = await _repository.ExistsWithEmailAsync(emailResult.Value, ct);
        if (emailExists)
        {
            _logger.LogWarning("Email already exists: {Email}", request.Email);
            return Result.Failure<UserDto>(
                Error.Conflict("User.Email.AlreadyExists", $"User with email '{request.Email}' already exists"));
        }

        // 创建用户聚合
        var userResult = User.Create(request.Email, request.FullName);
        if (userResult.IsFailure)
        {
            _logger.LogWarning("Failed to create user: {Error}", userResult.Error.Message);
            return Result.Failure<UserDto>(userResult.Error);
        }

        var user = userResult.Value;
        await _repository.AddAsync(user, ct);

        _logger.LogInformation("User created successfully with ID: {UserId}", user.Id);
        return Result.Success(ToDto(user));
    }

    /// <summary>
    /// 获取所有用户
    /// </summary>
    public async Task<IReadOnlyList<UserDto>> ListAsync(CancellationToken ct = default)
    {
        _logger.LogDebug("Retrieving all users from repository");

        var users = await _repository.ListAsync(ct);

        _logger.LogDebug("Retrieved {Count} users from repository", users.Count);
        return users.Select(ToDto).ToList();
    }

    /// <summary>
    /// 获取活跃用户
    /// </summary>
    public async Task<IReadOnlyList<UserDto>> GetActiveUsersAsync(CancellationToken ct = default)
    {
        _logger.LogDebug("Retrieving active users from repository");

        var users = await _repository.GetActiveUsersAsync(ct);

        _logger.LogDebug("Retrieved {Count} active users from repository", users.Count);
        return users.Select(ToDto).ToList();
    }

    /// <summary>
    /// 根据ID获取用户
    /// </summary>
    public async Task<Result<UserDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        _logger.LogDebug("Retrieving user with ID: {UserId}", id);

        var user = await _repository.GetByIdAsync(id, ct);
        if (user is null)
        {
            _logger.LogWarning("User with ID {UserId} not found", id);
            return Result.Failure<UserDto>(
                Error.NotFound("User.NotFound", $"User with ID '{id}' not found"));
        }

        _logger.LogDebug("User {UserId} retrieved successfully", id);
        return Result.Success(ToDto(user));
    }

    /// <summary>
    /// 根据邮箱获取用户
    /// </summary>
    public async Task<Result<UserDto>> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        _logger.LogDebug("Retrieving user with email: {Email}", email);

        var emailResult = Email.Create(email);
        if (emailResult.IsFailure)
        {
            return Result.Failure<UserDto>(emailResult.Error);
        }

        var user = await _repository.GetByEmailAsync(emailResult.Value, ct);
        if (user is null)
        {
            _logger.LogWarning("User with email {Email} not found", email);
            return Result.Failure<UserDto>(
                Error.NotFound("User.NotFound", $"User with email '{email}' not found"));
        }

        _logger.LogDebug("User with email {Email} retrieved successfully", email);
        return Result.Success(ToDto(user));
    }

    /// <summary>
    /// 更新用户
    /// </summary>
    public async Task<Result<UserDto>> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken ct = default)
    {
        _logger.LogInformation("Updating user {UserId}", id);

        var user = await _repository.GetByIdAsync(id, ct);
        if (user is null)
        {
            _logger.LogWarning("Cannot update user {UserId} - not found", id);
            return Result.Failure<UserDto>(
                Error.NotFound("User.NotFound", $"User with ID '{id}' not found"));
        }

        // 更新邮箱
        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var changeEmailResult = user.ChangeEmail(request.Email);
            if (changeEmailResult.IsFailure)
            {
                _logger.LogWarning("Failed to change email for user {UserId}: {Error}", id, changeEmailResult.Error.Message);
                return Result.Failure<UserDto>(changeEmailResult.Error);
            }
        }

        // 更新全名
        if (!string.IsNullOrWhiteSpace(request.FullName))
        {
            var updateNameResult = user.UpdateFullName(request.FullName);
            if (updateNameResult.IsFailure)
            {
                _logger.LogWarning("Failed to update full name for user {UserId}: {Error}", id, updateNameResult.Error.Message);
                return Result.Failure<UserDto>(updateNameResult.Error);
            }
        }

        _logger.LogInformation("User {UserId} updated successfully", id);
        return Result.Success(ToDto(user));
    }

    /// <summary>
    /// 停用用户
    /// </summary>
    public async Task<Result> DeactivateAsync(Guid id, DeactivateUserRequest request, CancellationToken ct = default)
    {
        _logger.LogInformation("Deactivating user {UserId}", id);

        var user = await _repository.GetByIdAsync(id, ct);
        if (user is null)
        {
            _logger.LogWarning("Cannot deactivate user {UserId} - not found", id);
            return Result.Failure(
                Error.NotFound("User.NotFound", $"User with ID '{id}' not found"));
        }

        var result = user.Deactivate(request.Reason);
        if (result.IsFailure)
        {
            _logger.LogWarning("Failed to deactivate user {UserId}: {Error}", id, result.Error.Message);
            return result;
        }

        _logger.LogInformation("User {UserId} deactivated successfully", id);
        return Result.Success();
    }

    /// <summary>
    /// 激活用户
    /// </summary>
    public async Task<Result> ActivateAsync(Guid id, CancellationToken ct = default)
    {
        _logger.LogInformation("Activating user {UserId}", id);

        var user = await _repository.GetByIdAsync(id, ct);
        if (user is null)
        {
            _logger.LogWarning("Cannot activate user {UserId} - not found", id);
            return Result.Failure(
                Error.NotFound("User.NotFound", $"User with ID '{id}' not found"));
        }

        var result = user.Activate();
        if (result.IsFailure)
        {
            _logger.LogWarning("Failed to activate user {UserId}: {Error}", id, result.Error.Message);
            return result;
        }

        _logger.LogInformation("User {UserId} activated successfully", id);
        return Result.Success();
    }

    /// <summary>
    /// 删除用户
    /// </summary>
    public async Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        _logger.LogInformation("Deleting user {UserId}", id);

        var user = await _repository.GetByIdAsync(id, ct);
        if (user is null)
        {
            _logger.LogWarning("Cannot delete user {UserId} - not found", id);
            return Result.Failure(
                Error.NotFound("User.NotFound", $"User with ID '{id}' not found"));
        }

        await _repository.RemoveAsync(user, ct);

        _logger.LogInformation("User {UserId} deleted successfully", id);
        return Result.Success();
    }

    private static UserDto ToDto(User user) =>
        new(user.Id, user.Email.Value, user.FullName, user.IsActive, user.CreatedAt, user.LastLoginAt);
}
