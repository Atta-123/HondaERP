using HondaERP.Application.DTOs.Auth;
using HondaERP.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HondaERP.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service)
    {
        _service = service;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _service.LoginAsync(request);

        if (result is null)
        {
            return Unauthorized(new
            {
                message = "Invalid credentials or account unavailable."
            });
        }

        return Ok(result);
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new
        {
            userId = User.FindFirst("sub")?.Value,
            fullName = User.FindFirst("name")?.Value,
            branchId = User.FindFirst("branchId")?.Value,
            roles = User.FindAll("role")
                .Select(x => x.Value)
                .ToArray()
        });
    }
}