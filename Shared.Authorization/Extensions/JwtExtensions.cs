using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Shared.Authorization.Extensions;

public static class JwtExtensions
{
    public static IServiceCollection AddJwtAuth(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        var key = config["Jwt:Key"];
        var issuer = config["Jwt:Issuer"];
        var audience = config["Jwt:Audience"];

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),

                    ClockSkew = TimeSpan.Zero,
                };
            });

        return services;
    }
}
