using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Common.Interceptors;
using Shared.InterHelperService.Contracts;
using Shared.InterHelperService.Protos;
using Shared.InterHelperService.Services;

namespace Shared.InterHelperService.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInterServiceHelper(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services
            .AddGrpcClient<UserGrpcService.UserGrpcServiceClient>(options =>
            {
                options.Address = new Uri(configuration["GrpcSettings:UserServiceUrl"]!);
            })
            .AddInterceptor<CorrelationIdGrpcInterceptor>();

        services.AddScoped<IUserHelperService, UserHelperService>();

        return services;
    }
}
