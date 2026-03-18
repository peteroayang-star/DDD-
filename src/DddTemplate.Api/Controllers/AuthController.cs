using Microsoft.AspNetCore.Mvc;
using DddTemplate.Application.Auth;
using DddTemplate.Api.Common;

namespace DddTemplate.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken ct)
    {
        var result = await _authService.LoginAsync(request, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<LoginResponse>.SuccessResponse(result.Value))
            : Unauthorized(ApiResponse<LoginResponse>.FailureResponse(
                new ErrorDetails(result.Error.Code, result.Error.Message, result.Error.Type.ToString())));
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken ct)
    {
        var result = await _authService.RegisterAsync(request, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<Guid>.SuccessResponse(result.Value))
            : BadRequest(ApiResponse<Guid>.FailureResponse(
                new ErrorDetails(result.Error.Code, result.Error.Message, result.Error.Type.ToString())));
    }
}
