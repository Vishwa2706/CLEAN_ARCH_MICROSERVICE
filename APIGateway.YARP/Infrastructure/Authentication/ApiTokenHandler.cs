using System.Security.Claims;
using System.Text.Encodings.Web;
using APIGateway.YARP.Contracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace APIGateway.YARP.Infrastructure.Authentication
{
    public class ApiTokenHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly ITokenValidator _validator;

        public ApiTokenHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            ITokenValidator validator
        )
            : base(options, logger, encoder, clock)
        {
            _validator = validator;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (Request.Path.StartsWithSegments("/api/auth"))
                return AuthenticateResult.NoResult();

            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Missing Authorization Header");

            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var (isValid, jwtToken, error) = await _validator.ValidateAsync(token);

            if (!isValid)
                return AuthenticateResult.Fail(error);

            var claims = jwtToken.Claims.Select(c => new Claim(c.Type, c.Value)).ToList();

            foreach (var claim in claims)
            {
                Logger.LogInformation($"Claim: {claim.Type} = {claim.Value}");
            }

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);

            return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
        }
    }
}
