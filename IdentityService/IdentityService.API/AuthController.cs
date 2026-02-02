using System.ComponentModel.DataAnnotations;
using IdentityService.Application.Contracts;
using IdentityService.Application.Dtos;
using IdentityService.Application.Services;
using IdentityService.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        await _authService.RegisterAsync(request);
        return Ok("User Registered");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        return Ok(result);
    }
}
