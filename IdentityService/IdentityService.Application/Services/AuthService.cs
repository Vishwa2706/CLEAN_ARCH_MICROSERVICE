using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using IdentityService.Application.Contracts;
using IdentityService.Application.Dtos;
using IdentityService.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace IdentityService.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserService _repo;
    private readonly IConfiguration _config;

    public AuthService(IUserService repo, IConfiguration config)
    {
        _repo = repo;
        _config = config;
    }

    public async Task RegisterAsync(RegisterRequest request)
    {
        var existing = await _repo.GetByMobileAsync(request.Mobile);
        if (existing != null)
            throw new Exception("User exists");

        var hash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Name = request.Name,
            Mobile = request.Mobile,
            Role = request.Role,
            PasswordHash = hash,
        };

        await _repo.AddAsync(user);
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _repo.GetByMobileAsync(request.Mobile);
        if (user == null)
            throw new Exception("Invalid credentials");

        bool valid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!valid)
            throw new Exception("Invalid credentials");

        string token = GenerateJwt(user);

        return new LoginResponse { Token = token };
    }

    private string GenerateJwt(User user)
    {
        var claims = new[]
        {
            new Claim("UserId", user.Id.ToString()),
            new Claim("Role", user.Role),
            new Claim("Name", user.Name),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:DurationInMinutes"])),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
