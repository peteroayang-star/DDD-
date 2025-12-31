using DddTemplate.Domain.Abstractions;
using DddTemplate.Domain.Users.Events;
using DddTemplate.Domain.Users.ValueObjects;

namespace DddTemplate.Domain.Users;

/// <summary>
/// User 聚合根
/// 演示完整的DDD聚合根实现，包括：
/// - 值对象使用（Email）
/// - 领域事件发布
/// - 业务规则封装
/// - Result模式错误处理
/// </summary>
public sealed class User : AggregateRoot<Guid>
{
    public Email Email { get; private set; }
    public string FullName { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    // EF Core 需要的无参构造函数
    private User() : base(Guid.Empty)
    {
        Email = null!;
        FullName = string.Empty;
    }

    private User(Guid id, Email email, string fullName) : base(id)
    {
        Email = email;
        FullName = fullName;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 创建新用户（工厂方法）
    /// </summary>
    public static Result<User> Create(string email, string fullName)
    {
        // 验证全名
        if (string.IsNullOrWhiteSpace(fullName))
        {
            return Result.Failure<User>(
                Error.Validation("User.FullName.Empty", "Full name cannot be empty"));
        }

        if (fullName.Length > 100)
        {
            return Result.Failure<User>(
                Error.Validation("User.FullName.TooLong", "Full name cannot exceed 100 characters"));
        }

        // 创建Email值对象
        var emailResult = Email.Create(email);
        if (emailResult.IsFailure)
        {
            return Result.Failure<User>(emailResult.Error);
        }

        var user = new User(Guid.NewGuid(), emailResult.Value, fullName.Trim());

        // 发布领域事件
        user.AddDomainEvent(new UserCreatedEvent(user.Id, user.Email.Value, user.FullName));

        return Result.Success(user);
    }

    /// <summary>
    /// 更改邮箱
    /// </summary>
    public Result ChangeEmail(string newEmail)
    {
        var emailResult = Email.Create(newEmail);
        if (emailResult.IsFailure)
        {
            return Result.Failure(emailResult.Error);
        }

        if (Email.Equals(emailResult.Value))
        {
            return Result.Failure(
                Error.Validation("User.Email.SameAsOld", "New email is the same as the current email"));
        }

        var oldEmail = Email.Value;
        Email = emailResult.Value;

        // 发布领域事件
        AddDomainEvent(new UserEmailChangedEvent(Id, oldEmail, Email.Value));

        return Result.Success();
    }

    /// <summary>
    /// 更新全名
    /// </summary>
    public Result UpdateFullName(string newFullName)
    {
        if (string.IsNullOrWhiteSpace(newFullName))
        {
            return Result.Failure(
                Error.Validation("User.FullName.Empty", "Full name cannot be empty"));
        }

        if (newFullName.Length > 100)
        {
            return Result.Failure(
                Error.Validation("User.FullName.TooLong", "Full name cannot exceed 100 characters"));
        }

        FullName = newFullName.Trim();
        return Result.Success();
    }

    /// <summary>
    /// 停用用户
    /// </summary>
    public Result Deactivate(string reason)
    {
        if (!IsActive)
        {
            return Result.Failure(
                Error.Conflict("User.AlreadyDeactivated", "User is already deactivated"));
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            return Result.Failure(
                Error.Validation("User.Deactivate.ReasonRequired", "Deactivation reason is required"));
        }

        IsActive = false;

        // 发布领域事件
        AddDomainEvent(new UserDeactivatedEvent(Id, reason));

        return Result.Success();
    }

    /// <summary>
    /// 激活用户
    /// </summary>
    public Result Activate()
    {
        if (IsActive)
        {
            return Result.Failure(
                Error.Conflict("User.AlreadyActive", "User is already active"));
        }

        IsActive = true;

        // 发布领域事件
        AddDomainEvent(new UserActivatedEvent(Id));

        return Result.Success();
    }

    /// <summary>
    /// 记录登录时间
    /// </summary>
    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }
}
