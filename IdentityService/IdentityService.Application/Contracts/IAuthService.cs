using System.Linq;
using IdentityService.Application.Dtos;
using IdentityService.Domain.Entities;

namespace IdentityService.Application.Contracts;

public interface IAuthService
{
    Task RegisterAsync(RegisterRequest request);
    Task<LoginResponse> LoginAsync(LoginRequest request);
}
