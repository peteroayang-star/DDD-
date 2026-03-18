using DddTemplate.Domain.Users;

namespace DddTemplate.Domain.Tests.Users;

public class UserTests
{
    [Fact]
    public void Create_ShouldCreateUser_WithValidData()
    {
        var email = "test@example.com";
        var fullName = "Test User";

        var result = User.Create(email, fullName);

        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value.Id);
        Assert.Equal(fullName, result.Value.FullName);
        Assert.True(result.Value.IsActive);
    }

    [Fact]
    public void Create_ShouldFail_WithInvalidEmail()
    {
        var result = User.Create("invalid-email", "Test User");

        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalse()
    {
        var user = User.Create("test@example.com", "Test User").Value;

        user.Deactivate("Test reason");

        Assert.False(user.IsActive);
    }
}
