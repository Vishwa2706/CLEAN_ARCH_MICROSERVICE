using System.IdentityModel.Tokens.Jwt;
using APIGateway.YARP.Contracts;

namespace APIGateway.YARP.Infrastructure.Services
{
    public class JwtTokenValidator : ITokenValidator
    {
        public async Task<(bool IsValid, JwtSecurityToken JwtToken, string Error)> ValidateAsync(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            // 🔹 Step 1: Check format
            if (!handler.CanReadToken(token))
                return (false, null, "Invalid token format");

            var jwtToken = handler.ReadJwtToken(token);

            // 🔹 Step 2: Expiry check
            if (jwtToken.ValidTo < DateTime.UtcNow)
                return (false, null, "Token expired");

            // 🔥 Step 3: IDENTITY SERVICE VALIDATION (PLACEHOLDER)
            // ---------------------------------------------------
            // This is EXACTLY like your zip logic
            //
            // bool isValidFromIdentity = await ValidateFromIdentityService(token);
            //
            // if (!isValidFromIdentity)
            //     return (false, null, "Invalid token from IdentityService");
            // ---------------------------------------------------

            return (true, jwtToken, null);
        }

        private async Task<bool> ValidateFromIdentityService(string token)
        {
            // 🔥 Future implementation (same as your old project)
            //
            // HttpClient call:
            // POST /validate-token
            //
            // return response.IsValid;

            return true;
        }
    }
}