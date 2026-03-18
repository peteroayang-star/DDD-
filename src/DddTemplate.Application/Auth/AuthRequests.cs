namespace DddTemplate.Application.Auth;

public sealed record LoginRequest(string Email, string Password);

public sealed record LoginResponse(string Token, string Email, string FullName);

public sealed record RegisterRequest(string Email, string Password, string FullName);
