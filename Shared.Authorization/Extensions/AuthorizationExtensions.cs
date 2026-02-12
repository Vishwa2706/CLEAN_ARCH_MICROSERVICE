using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Shared.Authorization.Constants;
using Shared.Authorization.Handlers;
using Shared.Authorization.Requirements;

namespace Shared.Authorization.Extensions;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddPermissionAuth(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(
                Permissions.ExpenseRead,
                p => p.Requirements.Add(new PermissionRequirement(Permissions.ExpenseRead))
            );

            options.AddPolicy(
                Permissions.ExpenseCreate,
                p => p.Requirements.Add(new PermissionRequirement(Permissions.ExpenseCreate))
            );

            options.AddPolicy(
                Permissions.ExpenseDelete,
                p => p.Requirements.Add(new PermissionRequirement(Permissions.ExpenseDelete))
            );
        });

        services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

        return services;
    }
}
