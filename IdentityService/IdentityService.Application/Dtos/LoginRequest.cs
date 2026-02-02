namespace IdentityService.Application.Dtos;

public class LoginRequest
{
    public long Mobile { get; set; }
    public string Password { get; set; } = null!;
}
