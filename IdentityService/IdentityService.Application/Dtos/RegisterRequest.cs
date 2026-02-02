namespace IdentityService.Application.Dtos;

public class RegisterRequest
{
    public string Name { get; set; } = null!;
    public long Mobile { get; set; }
    public string Password { get; set; } = null!;
    public string Role { get; set; } = "Member";
}
