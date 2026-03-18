using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using DddTemplate.Domain.Abstractions;
using DddTemplate.Domain.Users;
using DddTemplate.Domain.Users.ValueObjects;

namespace DddTemplate.Application.Auth;

public sealed class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure)
        {
            return Result.Failure<LoginResponse>(emailResult.Error);
        }

        var user = await _userRepository.GetByEmailAsync(emailResult.Value, ct);
        if (user is null)
        {
            return Result.Failure<LoginResponse>(Error.Unauthorized("Auth.InvalidCredentials", "Invalid email or password"));
        }

        if (!user.IsActive)
        {
            return Result.Failure<LoginResponse>(Error.Forbidden("Auth.UserInactive", "User account is inactive"));
        }

        var token = GenerateJwtToken(user);
        return Result.Success(new LoginResponse(token, user.Email.Value, user.FullName));
    }

    public async Task<Result<Guid>> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure)
        {
            return Result.Failure<Guid>(emailResult.Error);
        }

        var exists = await _userRepository.ExistsWithEmailAsync(emailResult.Value, ct);
        if (exists)
        {
            return Result.Failure<Guid>(Error.Conflict("Auth.EmailExists", "Email already registered"));
        }

        var userResult = User.Create(request.Email, request.FullName);
        if (userResult.IsFailure)
        {
            return Result.Failure<Guid>(userResult.Error);
        }

        await _userRepository.AddAsync(userResult.Value, ct);
        return Result.Success(userResult.Value.Id);
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "your-secret-key-min-32-chars!!"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email.Value),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
