using System.IdentityModel.Tokens.Jwt;

namespace APIGateway.YARP.Contracts
{
    public interface ITokenValidator
    {
        Task<(bool IsValid, JwtSecurityToken JwtToken, string Error)> ValidateAsync(string token);
    }
}