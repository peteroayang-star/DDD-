using System.Text.RegularExpressions;
using DddTemplate.Domain.Abstractions;

namespace DddTemplate.Domain.Users.ValueObjects;

/// <summary>
/// Email 值对象
/// 演示如何创建值对象并进行验证
/// </summary>
public sealed class Email : ValueObject
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    /// <summary>
    /// 创建Email值对象（使用Result模式）
    /// </summary>
    public static Result<Email> Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return Result.Failure<Email>(
                Error.Validation("Email.Empty", "Email cannot be empty"));
        }

        email = email.Trim().ToLowerInvariant();

        if (email.Length > 255)
        {
            return Result.Failure<Email>(
                Error.Validation("Email.TooLong", "Email cannot exceed 255 characters"));
        }

        if (!EmailRegex.IsMatch(email))
        {
            return Result.Failure<Email>(
                Error.Validation("Email.InvalidFormat", "Email format is invalid"));
        }

        return Result.Success(new Email(email));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
