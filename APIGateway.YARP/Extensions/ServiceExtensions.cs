using APIGateway.YARP.Contracts;
using APIGateway.YARP.Infrastructure.Authentication;
using APIGateway.YARP.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication;

namespace APIGateway.YARP.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddGatewayServices(this IServiceCollection services)
        {
            services.AddSingleton<ITokenValidator, JwtTokenValidator>();

            services
                .AddAuthentication("ApiTokenScheme")
                .AddScheme<AuthenticationSchemeOptions, ApiTokenHandler>("ApiTokenScheme", null);

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Authenticated", policy => policy.RequireAuthenticatedUser());
            });

            return services;
        }
    }
}
